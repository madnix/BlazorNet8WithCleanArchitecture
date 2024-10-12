using Application.DependencyInjection;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NetcodeHub.Packages.Components.DataGrid;
using NetcodeHub.Packages.Components.Toast;
using WebUI;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddApplicationServices();
builder.Services.AddScoped<ToastModel>();
builder.Services.AddScoped<NetcodeHubToast>();
builder.Services.AddVirtualizationService();
await builder.Build().RunAsync();