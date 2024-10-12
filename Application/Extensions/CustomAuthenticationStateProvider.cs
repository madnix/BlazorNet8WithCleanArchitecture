using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.DTOs.Request.Account;
using Application.DTOs.Response.Account;
using Microsoft.AspNetCore.Components.Authorization;

namespace Application.Extensions;

public class CustomAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    
    /// <summary>
    /// 사용자 클레임 설정
    /// </summary>
    /// <param name="claims"></param>
    /// <returns></returns>
    private static ClaimsPrincipal SetClaimPrincipal(UserClaimsDto? claims)
    {
        if (claims?.Email is null)
            return new ClaimsPrincipal();

        return new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, claims.UserName!),
            new Claim(ClaimTypes.Email, claims.Email),
            new Claim(ClaimTypes.Role, claims.Role!),
            new Claim("FullName", claims.FullName!)
        ], Constant.AuthenticationType));
    }

    /// <summary>
    /// Token 해석
    /// </summary>
    /// <param name="jwtToken"></param>
    /// <returns></returns>
    private static UserClaimsDto? DecryptToken(string? jwtToken)
    {
        try
        {
            if (string.IsNullOrEmpty(jwtToken))
                return new UserClaimsDto();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            
            var name = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var email = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var role = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var fullName = token.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
            return new UserClaimsDto(fullName!, name!, email!, role!);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 인증 정보 가져오기
    /// </summary>
    /// <returns></returns>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var accessToken = await localStorageService.GetModelFromToken();
        
        if (string.IsNullOrEmpty(accessToken?.Token))
            return await Task.FromResult(new AuthenticationState(_anonymous));
        
        var getUserClaims = DecryptToken(accessToken.Token);
        
        if (getUserClaims is null)
            return await Task.FromResult(new AuthenticationState(_anonymous));

        var claimsPrincipal = SetClaimPrincipal(getUserClaims);
        
        return await Task.FromResult(new AuthenticationState(claimsPrincipal));
    }

    /// <summary>
    /// 인증정보 갱신
    /// </summary>
    /// <param name="localStorageDto"></param>
    public async Task UpdateAuthenticationState(LocalStorageDto localStorageDto)
    {
        var claimsPrincipal = new ClaimsPrincipal();

        if (localStorageDto.Token is not null || localStorageDto.Refresh is not null)
        {
            await localStorageService.SetBrowserLocalStorage(localStorageDto);
            var getUserClaims = DecryptToken(localStorageDto.Token);
            claimsPrincipal = SetClaimPrincipal(getUserClaims);
        }
        else
        {
            await localStorageService.RemoveTokenFromBrowserLocalStorage();
        }
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}