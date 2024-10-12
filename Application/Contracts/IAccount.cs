using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;

namespace Application.Contracts;

public interface IAccount
{
    Task CreateAdmin();
    Task<GeneralResponse> CreateAccountAsync(CreateAccountDto model);
    Task<LoginResponse> LoginAccountAsync(LoginDto model);
    Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto model);
    Task<GeneralResponse> CreateRoleAsync(CreateRoleDto model);
    Task<IEnumerable<GetRoleDto>> GetRolesAsync();
    Task<IEnumerable<GetUsersWithRolesResponseDto>?> GetUsersWithRolesAsync();
    Task<GeneralResponse> ChangeUserRoleAsync(ChangeUserRoleRequestDto model);
}