namespace Application.DTOs.Request.Account;

public record ChangeUserRoleRequestDto(string UserEmail, string RoleName);