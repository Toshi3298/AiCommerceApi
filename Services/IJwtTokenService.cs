using AiCommerceApi.Models;

namespace AiCommerceApi.Services;

public interface IJwtTokenService
{
    string CreateToken(AppUser user, DateTime expiresAt);
}