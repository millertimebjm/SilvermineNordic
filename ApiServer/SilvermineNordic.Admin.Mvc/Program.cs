using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SilvermineNordic.Repository.Services;
using Microsoft.Extensions.DependencyInjection;
using SilvermineNordic.Repository;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;

public class Program 
{
    public static void Main(string[] args)
    {
        const string _applicationNameConfigurationService = "SilvermineNordic";
        const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

        var builder = WebApplication.CreateBuilder(args);

        builder
            .Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        string appConfigConnectionString =
                    // Windows config value
                    builder.Configuration[_appConfigEnvironmentVariableName]
                    // Linux config value
                    ?? builder.Configuration[$"Values:{_appConfigEnvironmentVariableName}"]
                    ?? throw new ArgumentNullException(_appConfigEnvironmentVariableName);

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddAzureAppConfiguration(options => 
            {
                options.Connect(appConfigConnectionString);
                options.ConfigureKeyVault(kv =>
                {
                    kv.SetCredential(new DefaultAzureCredential());
                });
            })
            .Build();

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<SilvermineNordicDbContext>();
        builder.Services.AddScoped<ISilvermineNordicDbContextFactory, SilvermineNordicDbContextFactory>();
        builder.Services.AddScoped<IWeatherForecast, OpenWeatherApiForecastService>();
        builder.Services.AddScoped<IRepositoryReading, EntityFrameworkReadingService>();
        builder.Services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
        builder.Services.AddScoped<ISms, AzureSmsService>();
        builder.Services.AddScoped<SilvermineNordicDbContextFactory>();
        builder.Services.AddOptions<SilvermineNordicConfigurationService>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                //var certificateUri = configuration["SilvermineNordic:CertificatePfx"];
                //var certificate = GetCertificateFromKeyVault(certificateUri);
                //settings.CertificatePfx = certificate;
                configuration.GetSection(_applicationNameConfigurationService).Bind(settings);
            });
        builder.Services.AddHttpClient();
        builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            }));



        // Create the X509Certificate2 object
        X509Certificate2 cert = new X509Certificate2("wildcard.bltmiller.com.pfx", "certificate");
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(9443, httpsOptions =>
            {
                httpsOptions.UseHttps(cert);
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }

    // private X509Certificate2 GetCertificateFromKeyVault(string keyVaultCertificateUri)
    // {
    //     var client = new CertificateClient(new Uri(keyVaultCertificateUri), new DefaultAzureCredential());
    //     var certificateBundle = client.GetCertificateAsync(keyVaultCertificateUri).Result;
    //     var pfxBytes = certificateBundle.Certificate.Export(X509ContentType.Pkcs12);
    //     return new X509Certificate2(pfxBytes, (string)null, X509KeyStorageFlags.MachineKeySet);
    // }

    private static TokenCredential GetAzureCredentials()
    {
        var isDeployed = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME"));
        return new DefaultAzureCredential(
            new DefaultAzureCredentialOptions
            {
                // Prevent deployed instances from trying things that don't work and generally take too long
                ExcludeInteractiveBrowserCredential = isDeployed,
                ExcludeVisualStudioCodeCredential = isDeployed,
                ExcludeVisualStudioCredential = isDeployed,
                ExcludeSharedTokenCacheCredential = isDeployed,
                ExcludeAzureCliCredential = isDeployed,
                ExcludeManagedIdentityCredential = false,
                Retry =
                {
                    // Reduce retries and timeouts to get faster failures
                    MaxRetries = 2,
                    NetworkTimeout = TimeSpan.FromSeconds(5),
                    MaxDelay = TimeSpan.FromSeconds(5)
                },

                // this helps devs use the right tenant
                InteractiveBrowserTenantId = DefaultTenantId,
                SharedTokenCacheTenantId = DefaultTenantId,
                VisualStudioCodeTenantId = DefaultTenantId,
                VisualStudioTenantId = DefaultTenantId
            }
        );
    }
}
