using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SilvermineNordic.Admin;
using SilvermineNordic.Repository;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<ISilvermineNordicConfiguration>(_ => new SilvermineNordicConfigurationService()
{
    SilvermineNordicApiUrl = builder.Configuration["SilvermineNordicApiUrl"],
});

await builder.Build().RunAsync();
