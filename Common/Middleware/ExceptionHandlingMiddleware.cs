using AiCommerceApi.Common.Responses;
using FluentValidation;

namespace AiCommerceApi.Common.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionHandlingMiddleware>
        _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException exception)
        {
            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            var errors = exception.Errors
                .Select(error => error.ErrorMessage)
                .Distinct()
                .ToArray();

            var response =
                ApiResponse<object?>.Fail(
                    errors,
                    "Doğrulama hatası.");

            await context.Response.WriteAsJsonAsync(
                response);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Beklenmeyen bir hata oluştu.");

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            var response =
                ApiResponse<object?>.Fail(
                    "Sunucu tarafında beklenmeyen bir hata oluştu.");

            await context.Response.WriteAsJsonAsync(
                response);
        }
    }
}