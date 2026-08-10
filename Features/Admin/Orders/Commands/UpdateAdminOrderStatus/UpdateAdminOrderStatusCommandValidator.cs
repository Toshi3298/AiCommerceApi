using AiCommerceApi.Models;
using FluentValidation;

namespace AiCommerceApi.Features.Admin.Orders.Commands
    .UpdateAdminOrderStatus;

public class UpdateAdminOrderStatusCommandValidator
    : AbstractValidator<UpdateAdminOrderStatusCommand>
{
    public UpdateAdminOrderStatusCommandValidator()
    {
        RuleFor(command => command.OrderId)
            .GreaterThan(0)
            .WithMessage(
                "Geçerli bir sipariş ID değeri gönderilmelidir.");

        RuleFor(command => command.Status)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(
                "Sipariş durumu boş bırakılamaz.")
            .Must(status =>
                Enum.TryParse<OrderStatus>(
                    status,
                    true,
                    out var parsedStatus) &&
                Enum.IsDefined(parsedStatus))
            .WithMessage(
                "Geçerli bir sipariş durumu gönderilmelidir.");
    }
}