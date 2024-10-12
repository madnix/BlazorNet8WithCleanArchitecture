using System.Net;
using System.Net.Http.Headers;
using Application.DTOs.Request.Account;
using Application.Services;
using Microsoft.AspNetCore.Components;

namespace Application.Extensions;

public class CustomHttpHandler (LocalStorageService localStorageService, 
    NavigationManager navigationManager,
    IAccountService accountService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var loginUrl = request.RequestUri != null && request.RequestUri.AbsoluteUri.Contains(Constant.LoginRoute);
            var registerUrl = request.RequestUri != null && request.RequestUri.AbsoluteUri.Contains(Constant.RegisterRoute);
            var refreshTokenUrl = request.RequestUri != null && request.RequestUri.AbsoluteUri.Contains(Constant.RegisterRoute);
            var adminCreateUrl = request.RequestUri != null && request.RequestUri.AbsoluteUri.Contains(Constant.CreateAdminRoute);
            
            if (loginUrl || registerUrl || refreshTokenUrl || adminCreateUrl)
                return await base.SendAsync(request, cancellationToken);
            
            var result = await base.SendAsync(request, cancellationToken);

            if (result.StatusCode != HttpStatusCode.Unauthorized) return result;
            var tokenModel = await localStorageService.GetModelFromToken();
            if (tokenModel is null)
                return result;

            if (tokenModel.Refresh == null) return await base.SendAsync(request, cancellationToken);
            var newJwtToken = await GetRefreshTokenAsync(tokenModel.Refresh);
            if (string.IsNullOrEmpty(newJwtToken))
                return result;
                
            request.Headers.Authorization = new AuthenticationHeaderValue(Constant.HttpClientHeaderScheme, newJwtToken);

            return await base.SendAsync(request, cancellationToken);

        }
        catch
        {
            return null!;
        }
    }

    private async Task<string?> GetRefreshTokenAsync(string refreshToken)
    {
        try
        {
            var response = await accountService.RefreshTokenAsync(new RefreshTokenDto { Token = refreshToken });
            
            switch (response)
            {
                case null:
                    await ClearBrowserStorage();
                    NavigateToLogin();
                    return null;
                case { Token: null }:
                    await ClearBrowserStorage();
                    NavigateToLogin();
                    return null;
            }

            await localStorageService.RemoveTokenFromBrowserLocalStorage();
            await localStorageService.SetBrowserLocalStorage(new LocalStorageDto { Refresh = response.RefreshToken, Token = response.Token });
            return response.Token;
        }
        catch
        {
            return null;
        }
    }
    
    private void NavigateToLogin()
        => navigationManager.NavigateTo(navigationManager.BaseUri, true, true);

    private async Task ClearBrowserStorage()
        => await localStorageService.RemoveTokenFromBrowserLocalStorage();
}