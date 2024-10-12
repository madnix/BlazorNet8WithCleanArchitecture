namespace Domain.Entity.Authentication;

public class RefreshToken
{
    public int Id { get; init; }
    
    public string? UserId { get; init; }
    
    public string? Token { get; set; }
}