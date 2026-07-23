using AiCommerceApi.Dtos.Auth;
using AiCommerceApi.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using AiCommerceApi.Features.Auth.Commands.Login;
namespace AiCommerceApi.Controllers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequestDto request,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        var result = await _mediator.Send(
            command,
            cancellationToken);

        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Error
            });
        }

        return Ok(new
        {
            message = "Kullanıcı başarıyla oluşturuldu.",
            userId = result.UserId
        });
    }
    [HttpPost("login")]
public async Task<IActionResult> Login(
    LoginRequestDto request,
    CancellationToken cancellationToken)
{
    var command = new LoginCommand(
        request.Email,
        request.Password);

    var result = await _mediator.Send(
        command,
        cancellationToken);

    if (!result.Success)
    {
        return Unauthorized(new
        {
            message = result.Error
        });
    }

    return Ok(new
    {
        token = result.Token,
        expiresAt = result.ExpiresAt
    });
}

[Authorize]
[HttpGet("me")]
public IActionResult GetMe()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var fullName = User.FindFirstValue(ClaimTypes.Name);
    var email = User.FindFirstValue(ClaimTypes.Email);
    var role = User.FindFirstValue(ClaimTypes.Role);

    return Ok(new
    {
        userId,
        fullName,
        email,
        role
    });
}

}