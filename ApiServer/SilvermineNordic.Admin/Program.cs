using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SilvermineNordic.Admin;
using SilvermineNordic.Repository;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var configurationString = string.Join(",",builder.Configuration.AsEnumerable().Select(_ => $"{_.Key}:{_.Value}"));
builder.Services.AddSingleton<ISilvermineNordicConfiguration>(_ => new SilvermineNordicConfigurationService()
{
    SilvermineNordicApiUrl = builder.Configuration["SilvermineNordicApiUrl"],
    ConfigurationString = configurationString,
});

await builder.Build().RunAsync();
