using AiCommerceApi.Data;
using AiCommerceApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AiCommerceApi.Features.Admin.Orders.Commands
    .UpdateAdminOrderStatus;

public class UpdateAdminOrderStatusCommandHandler
    : IRequestHandler<
        UpdateAdminOrderStatusCommand,
        UpdateAdminOrderStatusResult>
{
    private readonly ApplicationDbContext _context;

    public UpdateAdminOrderStatusCommandHandler(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateAdminOrderStatusResult> Handle(
        UpdateAdminOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(
                order => order.Id == request.OrderId,
                cancellationToken);

        if (order is null)
        {
            return new UpdateAdminOrderStatusResult
            {
                Success = false,
                NotFound = true,
                Error = "Sipariş bulunamadı."
            };
        }

        if (!Enum.TryParse<OrderStatus>(
                request.Status.Trim(),
                true,
                out var newStatus))
        {
            return Failure(
                order.Id,
                "Geçerli bir sipariş durumu gönderilmelidir.");
        }

        if (order.Status == newStatus)
        {
            return Success(order.Id, order.Status);
        }

        if (!IsValidTransition(order.Status, newStatus))
        {
            return Failure(
                order.Id,
                $"{order.Status} durumundaki sipariş " +
                $"{newStatus} durumuna geçirilemez.");
        }

        order.Status = newStatus;

        await _context.SaveChangesAsync(
            cancellationToken);

        return Success(order.Id, order.Status);
    }

    private static bool IsValidTransition(
        OrderStatus currentStatus,
        OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending =>
                newStatus is
                    OrderStatus.Preparing or
                    OrderStatus.Cancelled,

            OrderStatus.Preparing =>
                newStatus is
                    OrderStatus.Shipped or
                    OrderStatus.Cancelled,

            OrderStatus.Shipped =>
                newStatus == OrderStatus.Delivered,

            OrderStatus.Delivered => false,

            OrderStatus.Cancelled => false,

            _ => false
        };
    }

    private static UpdateAdminOrderStatusResult Success(
        int orderId,
        OrderStatus status)
    {
        return new UpdateAdminOrderStatusResult
        {
            Success = true,
            OrderId = orderId,
            Status = status.ToString()
        };
    }

    private static UpdateAdminOrderStatusResult Failure(
        int orderId,
        string error)
    {
        return new UpdateAdminOrderStatusResult
        {
            Success = false,
            OrderId = orderId,
            Error = error
        };
    }
}