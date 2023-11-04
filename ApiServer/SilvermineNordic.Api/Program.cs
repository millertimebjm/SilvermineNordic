using SilvermineNordic.Repository;
using SilvermineNordic.Models;
using SilvermineNordic.Repository.Services;
using SilvermineNordic.Common;
using SilvermineNordic.Api.Services;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

const string _applicationNameConfigurationService = "SilvermineNordic";
const string _appConfigEnvironmentVariableName = "AppConfigConnectionString";

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

builder.Services.AddOptions<SilvermineNordicConfigurationService>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection(_applicationNameConfigurationService).Bind(settings);
    });
builder.Services.AddDbContext<SilvermineNordicDbContext>(ServiceLifetime.Scoped);
builder.Services.AddScoped<IRepositoryReading, EntityFrameworkReadingService>();
builder.Services.AddScoped<IRepositoryThreshold, EntityFrameworkThresholdService>();
builder.Services.AddScoped<IWeatherForecast, OpenWeatherApiForecastService>();
builder.Services.AddScoped<IRepositoryUser, EntityFrameworkUserService>();
builder.Services.AddScoped<IRepositoryUserOtp, EntityFrameworkUserOtpService>();
builder.Services.AddScoped<IApiMapper, ApiMapperService>();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddCors(o => o.AddPolicy("NUXT", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors("NUXT");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Console.WriteLine($"OpenWeatherForecastApiKey: |{configService.GetOpenWeatherApiKey()}|");
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var apiMapper = scope.ServiceProvider.GetRequiredService<IApiMapper>();
    apiMapper.SetMaps(app);
    app.Run();
}
