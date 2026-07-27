using FluentValidation;
using MediatR;

namespace AiCommerceApi.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>>
        _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var validationContext =
            new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(validator =>
                validator.ValidateAsync(
                    validationContext,
                    cancellationToken)));

        var validationErrors = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error is not null)
            .ToList();

        if (validationErrors.Count != 0)
        {
            throw new ValidationException(
                validationErrors);
        }

        return await next();
    }
}