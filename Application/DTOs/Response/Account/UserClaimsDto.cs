namespace Application.DTOs.Response.Account;

public record UserClaimsDto(
    string FullName = null!, 
    string UserName = null!, 
    string Email = null!, 
    string Role = null!
    );