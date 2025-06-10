using SilvermineNordic.Repository.Services;
using SilvermineNordic.Repository;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

const string _applicationNameConfigurationService = "SilvermineNordic";
const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

var builder = WebApplication.CreateBuilder(args);
builder
    .Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

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
    .AddAzureAppConfiguration(appConfigConnectionString);

builder.Host.UseSerilog();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.GrafanaLoki("http://media.bltmiller.com:3100", 
                labels: new[]
                {
                    new LokiLabel() {Key = "app", Value = "snowmaking"},
                    new LokiLabel() {Key = "env", Value = "prod"},
                })
            .CreateLogger();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IWeatherForecast, OpenWeatherApiForecastService>();
builder.Services.AddTransient<IRepositoryReading, EntityFrameworkReadingService>();
builder.Services.AddTransient<IRepositoryThreshold, EntityFrameworkThresholdService>();
builder.Services.AddScoped<ISms, AzureSmsService>();
builder.Services.AddOptions<SilvermineNordicConfigurationService>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection(_applicationNameConfigurationService).Bind(settings);
    });

var connectionString = builder.Configuration
    .GetValue(typeof(string), $"{_applicationNameConfigurationService}:SqlConnectionString")?
    .ToString();
Console.WriteLine($"Connection String: {connectionString}");
if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
builder.Services.AddDbContext<SilvermineNordicDbContext>(opts => 
    opts.UseNpgsql(connectionString, options => options.MigrationsAssembly("SilvermineNordic.Admin.Mvc"))
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information), ServiceLifetime.Transient);

builder.Services.AddHttpClient();
builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder
        .AllowAnyOrigin()
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
