namespace Shared.Dtos;

public class UserSessionDto
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
}