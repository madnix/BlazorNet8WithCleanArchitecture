using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;

namespace Application.Services;

public interface IAccountService
{
    Task CreateAdmin();
    Task<GeneralResponse?> RegisterAccountAsync(CreateAccountDto model);
    Task<LoginResponse?> LoginAccountAsync(LoginDto model);
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto model);
    Task<GeneralResponse?> CreateRoleAsync(CreateRoleDto model);
    IEnumerable<GetRoleDto> GetDefaultRoles();
    Task<IEnumerable<GetRoleDto>?> GetRolesAsync();
    Task<IEnumerable<GetUsersWithRolesResponseDto>?> GetUsersWithRolesAsync();
    Task<GeneralResponse?> ChangeUserRoleAsync(ChangeUserRoleRequestDto model);
}