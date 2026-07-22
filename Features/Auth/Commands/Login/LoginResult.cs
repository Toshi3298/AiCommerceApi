namespace AiCommerceApi.Features.Auth.Commands.Login;

public class LoginResult
{
    public bool Success { get; set; }

    public string? Token { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string? Error { get; set; }
}