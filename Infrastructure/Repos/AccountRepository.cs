using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Contracts;
using Application.DTOs.Request.Account;
using Application.DTOs.Response;
using Application.DTOs.Response.Account;
using Domain.Entity.Authentication;
using Infrastructure.Data;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using static Application.Extensions.Constant;

namespace Infrastructure.Repos;

public class AccountRepository (
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IConfiguration config,
    SignInManager<ApplicationUser> signInManager,
    AppDbContext context) : IAccount
{
    #region Private Methods

    /// <summary>
    /// Email로 사용자 정보 조회하기
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    private async Task<ApplicationUser?> FindUserByEmailAsync(string email)
        => await userManager.FindByEmailAsync(email);

    /// <summary>
    /// 사용자 이름으로 사용자 Role 조회하기
    /// </summary>
    /// <param name="roleName"></param>
    /// <returns></returns>
    private async Task<IdentityRole?> FindRoleByNameAsync(string roleName)
        => await roleManager.FindByNameAsync(roleName);
    
    /// <summary>
    /// 토큰 초기화
    /// </summary>
    /// <returns></returns>
    private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    /// <summary>
    /// 토큰 생성
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private async Task<string?> GenerateTokenAsync(ApplicationUser? user)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? string.Empty));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user?.Email ?? string.Empty),
                new Claim(ClaimTypes.Email, user?.Email ?? string.Empty),
                new Claim(ClaimTypes.Role, (await userManager.GetRolesAsync(user!)).FirstOrDefault() ?? string.Empty),
                new Claim("FullName", user?.Name ?? string.Empty)
            };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch
        {
            return null!;
        }
    }

    /// <summary>
    /// 사용자 Role 할당
    /// </summary>
    /// <param name="user"></param>
    /// <param name="role"></param>
    /// <returns></returns>
    private async Task<GeneralResponse> AssignUserToRole(ApplicationUser? user, IdentityRole? role)
    {
        if (user is null || role is null) return new GeneralResponse(false, "Model state cannot be empty");
        if (role.Name != null && await FindRoleByNameAsync(role.Name) is null)
            await CreateRoleAsync(role.Adapt(new CreateRoleDto()));

        if (role.Name == null) return new GeneralResponse();
        var result = await userManager.AddToRoleAsync(user, role.Name);
        var error = CheckResponse(result);
        return !string.IsNullOrEmpty(error)
            ? new GeneralResponse(false, error)
            : new GeneralResponse(true, $"{user.Name} assigned to {role.Name} role");

    }

    /// <summary>
    /// 반환값 체크
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    private static string? CheckResponse(IdentityResult result)
    {
        if (result.Succeeded) return null!;
        var error = result.Errors.Select(i => i.Description);
        return string.Join(Environment.NewLine, error);
    }

    /// <summary>
    /// 리프레시 토큰 DB에 저장
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private async Task<GeneralResponse> SaveRefreshTokenAsync(string userId, string token)
    {
        try
        {
            var user = await context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is null)
                context.RefreshTokens.Add(new RefreshToken { UserId = userId, Token = token });
            else
                user.Token = token;

            await context.SaveChangesAsync();
            return new GeneralResponse(true);
        }
        catch (Exception ex)
        {
            return new GeneralResponse(false, ex.Message);
        }
    }
    
    #endregion

    #region IAccount Implementation Methods

    /// <summary>
    /// 관리자 계정 생성
    /// </summary>
    public async Task CreateAdmin()
    {
        try
        {
            if (await FindRoleByNameAsync(Role.Admin).ConfigureAwait(false) is not null) return;
            var admin = new CreateAccountDto()
            {
                Name = "Admin",
                Password = "Admin@123",
                EmailAddress = "admin@admin.com",
                Role = Role.Admin
            };

            await CreateAccountAsync(admin);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// 사용자 계정 생성
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<GeneralResponse> CreateAccountAsync(CreateAccountDto model)
    {
        try
        {
            if (await FindUserByEmailAsync(model.EmailAddress).ConfigureAwait(false) is not null)
                return new GeneralResponse(false, "Sorry, user is already created");

            var user = new ApplicationUser()
            {
                Name = model.Name,
                UserName = model.EmailAddress,
                Email = model.EmailAddress,
                PasswordHash = model.Password
            };
            var result = await userManager.CreateAsync(user, model.Password);
            var error = CheckResponse(result);
            if (!string.IsNullOrEmpty(error))
                return new GeneralResponse(false, error);

            var (flag, message) = await AssignUserToRole(user, new IdentityRole()
            {
                Name = model.Role
            });
            return new GeneralResponse(flag, message);
        }
        catch (Exception ex)
        {
            return new GeneralResponse(false, ex.Message);
        }
    }

    /// <summary>
    /// 로그인
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<LoginResponse> LoginAccountAsync(LoginDto model)
    {
        try
        {
            var user = await FindUserByEmailAsync(model.EmailAddress);
            if (user is null)
                return new LoginResponse(false, "User not found");

            SignInResult result;
            try
            {
                result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            }
            catch
            {
                return new LoginResponse(false, "Invalid credentials");
            }
            
            if (!result.Succeeded)
                return new LoginResponse(false, "Invalid credentials");
            
            var jwtToken = await GenerateTokenAsync(user);
            var refreshToken = GenerateRefreshToken();
            
            if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(refreshToken))
                return new LoginResponse(false, "Error occured while logging in account, please contract administrator");
            
            var saveResult = await SaveRefreshTokenAsync(user.Id, refreshToken);
            return saveResult.Flag ? new LoginResponse(true, $"{user.Name} successfully logged in", jwtToken, refreshToken) : new LoginResponse();
        }
        catch (Exception ex)
        {
            return new LoginResponse(false, ex.Message);
        }
    }

    /// <summary>
    /// 토큰 리프레시
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<LoginResponse> RefreshTokenAsync(RefreshTokenDto model)
    {
        var token = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == model.Token);
        if (token is null)
            return new LoginResponse();

        if (token.UserId == null) return new LoginResponse();
        var user = await userManager.FindByIdAsync(token.UserId);
        var newToken = await GenerateTokenAsync(user);
        var newRefreshToken = GenerateRefreshToken();
        if (user?.Id == null) return new LoginResponse();
        var saveResult = await SaveRefreshTokenAsync(user.Id, newRefreshToken);
        return saveResult.Flag
            ? new LoginResponse(true, $"{user.Name} successfully re-logged in", newToken, newRefreshToken)
            : new LoginResponse();
    }

    /// <summary>
    /// Role 생성
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<GeneralResponse> CreateRoleAsync(CreateRoleDto model)
    {
        try
        {
            if (model.Name is null) 
                return new GeneralResponse(false, $"Name is null");
            
            if (await FindRoleByNameAsync(model.Name).ConfigureAwait(false) is not null)
                return new GeneralResponse(false, $"{model.Name} already role created");
            
            var response = await roleManager.CreateAsync(new IdentityRole(model.Name));
            var error = CheckResponse(response);
            
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            return new GeneralResponse(true, $"{model.Name} role created");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Role 목록 반환
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<GetRoleDto>> GetRolesAsync()
        => (await roleManager.Roles.ToListAsync()).Adapt<IEnumerable<GetRoleDto>>();

    /// <summary>
    /// 사용자 목록과 Role 목록을 반환
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<GetUsersWithRolesResponseDto>?> GetUsersWithRolesAsync()
    {
        var allUsers = await userManager.Users.ToListAsync();
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (allUsers is null) return null;
        
        var list = new List<GetUsersWithRolesResponseDto>();
        foreach (var user in allUsers)
        {
            var getUserRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
            var getRoleInfo = await roleManager.Roles.FirstOrDefaultAsync(r =>
                    r.Name != null && getUserRole != null && r.Name == getUserRole)
                .ConfigureAwait(false);

            list.Add(new GetUsersWithRolesResponseDto()
            {
                Name = user.Name,
                Email = user.Email,
                RoleId = getRoleInfo?.Id,
                RoleName = getRoleInfo?.Name
            });
        }

        return list;
    }

    /// <summary>
    /// 사용자 Role 변경
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<GeneralResponse> ChangeUserRoleAsync(ChangeUserRoleRequestDto model)
    {
        if (await FindRoleByNameAsync(model.RoleName) is null) 
            return new GeneralResponse(false, "Role not found");
        
        if (await FindUserByEmailAsync(model.UserEmail) is null)
            return new GeneralResponse(false, "User not found");

        var user = await FindUserByEmailAsync(model.UserEmail);
        if (user == null) 
            return new GeneralResponse(false, "User not found by email");
        
        var previousRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
        if (previousRole == null) 
            return new GeneralResponse(false, "Previous role is not found");
        
        var removeOldRole = await userManager.RemoveFromRoleAsync(user, previousRole);
        var error = CheckResponse(removeOldRole);
        if (!string.IsNullOrEmpty(error))
            return new GeneralResponse(false, error);
                
        var result = await userManager.AddToRoleAsync(user, model.RoleName);
        var response = CheckResponse(result);
        return !string.IsNullOrEmpty(error)
            ? new GeneralResponse(false, response)
            : new GeneralResponse(true, "Role changed");
    }
    
    #endregion
}