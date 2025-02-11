using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SilvermineNordic.Repository.Services;
using Microsoft.Extensions.DependencyInjection;
using SilvermineNordic.Repository;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

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
    .AddAzureAppConfiguration(appConfigConnectionString)
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
        configuration.GetSection(_applicationNameConfigurationService).Bind(settings);
    });
builder.Services.AddHttpClient();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
    {
        builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
    }));

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
