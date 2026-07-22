using AiCommerceApi.Data;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Auth.Commands.Register;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher<AppUser> _passwordHasher;

    public RegisterCommandHandler(
        ApplicationDbContext context,
        IPasswordHasher<AppUser> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        string normalizedEmail = request.Email.Trim().ToLowerInvariant();

        bool emailExists = await _context.Users.AnyAsync(
            user => user.Email == normalizedEmail,
            cancellationToken);

        if (emailExists)
        {
            return new RegisterResult
            {
                Success = false,
                Error = "Bu e-posta adresi zaten kullanılıyor."
            };
        }

        var user = new AppUser
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, request.Password);

        user.Cart = new Cart
        {
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync(cancellationToken);

        return new RegisterResult
        {
            Success = true,
            UserId = user.Id
        };
    }
}