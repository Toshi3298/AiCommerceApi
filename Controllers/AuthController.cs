using System.Security.Claims;
using AiCommerceApi.Common.Responses;
using AiCommerceApi.Dtos.Auth;
using AiCommerceApi.Features.Auth.Commands.Login;
using AiCommerceApi.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiCommerceApi.Controllers;

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
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "Kullanıcı oluşturulamadı.");

            return BadRequest(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                new
                {
                    userId = result.UserId
                },
                "Kullanıcı başarıyla oluşturuldu.");

        return StatusCode(
            StatusCodes.Status201Created,
            response);
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
            var errorResponse =
                ApiResponse<object?>.Fail(
                    result.Error
                    ?? "E-posta veya şifre hatalı.");

            return Unauthorized(errorResponse);
        }

        var response =
            ApiResponse<object?>.Ok(
                new
                {
                    token = result.Token,
                    expiresAt = result.ExpiresAt
                },
                "Giriş başarılı.");

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        var fullName =
            User.FindFirstValue(ClaimTypes.Name);

        var email =
            User.FindFirstValue(ClaimTypes.Email);

        var role =
            User.FindFirstValue(ClaimTypes.Role);

        var response =
            ApiResponse<object?>.Ok(
                new
                {
                    userId,
                    fullName,
                    email,
                    role
                },
                "Kullanıcı bilgileri başarıyla getirildi.");

        return Ok(response);
    }
}