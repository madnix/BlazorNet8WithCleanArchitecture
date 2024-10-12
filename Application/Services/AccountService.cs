using System.Net.Http.Json;
using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Application.Extensions;

namespace Application.Services;

public class AccountService(HttpClientService httpClientService) : IAccountService
{
    private static string? CheckResponseStatus(HttpResponseMessage response)
    {
        return !response.IsSuccessStatusCode
            ? $"Sorry unknown error occured.{Environment.NewLine}" +
              $"Error Description:{Environment.NewLine}" +
              $"Status Code: {response.StatusCode}{Environment.NewLine}" +
              $"Reason Phrase: {response.ReasonPhrase}"
            : null;
    }
    
    public async Task<LoginResponse?> LoginAccountAsync(LoginDto model)
    {
        try
        {
            var publicClient = httpClientService.GetPublicClient();
            var response = await publicClient.PostAsJsonAsync(Constant.LoginRoute, model);
            var error = CheckResponseStatus(response);

            if (!string.IsNullOrEmpty(error))
                return new LoginResponse(false, error);
            
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result;
        }
        catch (Exception ex)
        {
            return new LoginResponse(false, ex.Message);
        }
    }

    public async Task<GeneralResponse?> RegisterAccountAsync(CreateAccountDto model)
    {
        try
        {
            var publicClient = httpClientService.GetPublicClient();
            var response = await publicClient.PostAsJsonAsync(Constant.RegisterRoute, model);
            var error = CheckResponseStatus(response);
            
            if (!string.IsNullOrEmpty(error))
                return new GeneralResponse(false, error);
            
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result;
        }
        catch (Exception ex)
        {
            return new GeneralResponse(false, ex.Message);
        }
    }

    public async Task CreateAdminAtFirstStart()
    {
        try
        {
            var client = httpClientService.GetPublicClient();
            await client.PostAsync(Constant.CreateAdminRoute, null);
        }
        catch
        {
            // ignored
        }
    }

    public IEnumerable<GetRoleDto> GetDefaultRoles()
    {
        var list = new List<GetRoleDto>();
        list.Clear();
        list.Add(new GetRoleDto(1.ToString(), Constant.Role.Admin));
        list.Add(new GetRoleDto(2.ToString(), Constant.Role.User));
        return list;
    }

    public async Task<IEnumerable<GetRoleDto>?> GetRolesAsync()
    {
        try
        {
            var privateClient = await httpClientService.GetPrivateClient();
            var response = await privateClient.GetAsync(Constant.GetRolesRoute);
            var error = CheckResponseStatus(response);
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);
            
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetRoleDto>>();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public async Task<GeneralResponse?> CreateRoleAsync(CreateRoleDto model)
    {
        try
        {
            var publicClient = httpClientService.GetPublicClient();
            var response = await publicClient.PostAsJsonAsync(Constant.CreateRoleRoute, model);
            var error = CheckResponseStatus(response);
            
            if (!string.IsNullOrEmpty(error))
                return new GeneralResponse(false, error);
            
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result;
        }
        catch (Exception ex)
        {
            return new GeneralResponse(false, ex.Message);
        }
    }

    public async Task<IEnumerable<GetUsersWithRolesResponseDto>?> GetUsersWithRolesAsync()
    {
        try
        {
            var privateClient = await httpClientService.GetPrivateClient();
            var response = await privateClient.GetAsync(Constant.GetUserWithRolesRoute);
            var error = CheckResponseStatus(response);

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);
            
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<GetUsersWithRolesResponseDto>>();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<GeneralResponse?> ChangeUserRoleAsync(ChangeUserRoleRequestDto model)
    {
        try
        {
            var publicClient = httpClientService.GetPublicClient();
            var response = await publicClient.PostAsJsonAsync(Constant.ChangeUserRoleRoute, model);
            var error = CheckResponseStatus(response);

            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);
            
            var result = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
    
    public Task CreateAdmin()
    {
        throw new NotImplementedException();
    }
    
    public Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto model)
    {
        throw new NotImplementedException();
    }
}