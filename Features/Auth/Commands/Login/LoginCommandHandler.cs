using AiCommerceApi.Data;
using AiCommerceApi.Models;
using AiCommerceApi.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<AppUser> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        ApplicationDbContext context,
        IPasswordHasher<AppUser> passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResult> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        string normalizedEmail =
            request.Email.Trim().ToLowerInvariant();

        var user = await _context.Users.FirstOrDefaultAsync(
            user => user.Email == normalizedEmail,
            cancellationToken);

        if (user is null)
        {
            return InvalidCredentials();
        }

        var verificationResult =
            _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return InvalidCredentials();
        }

        DateTime expiresAt = DateTime.UtcNow.AddHours(2);

        string token =
            _jwtTokenService.CreateToken(user, expiresAt);

        return new LoginResult
        {
            Success = true,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    private static LoginResult InvalidCredentials()
    {
        return new LoginResult
        {
            Success = false,
            Error = "E-posta veya şifre hatalı."
        };
    }
}