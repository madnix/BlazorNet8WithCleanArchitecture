using Application.Extensions;
using Application.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NetcodeHub.Packages.Extensions.LocalStorage;

namespace Application.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddAuthorizationCore();
        services.AddNetcodeHubLocalStorageService();
        services.AddScoped<Extensions.LocalStorageService>();
        services.AddScoped<HttpClientService>();
        services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        services.AddTransient<CustomHttpHandler>();
        services.AddCascadingAuthenticationState();
        services.AddHttpClient("WebUIClient", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7257");
        }).AddHttpMessageHandler<CustomHttpHandler>();
        return services;
    }
}