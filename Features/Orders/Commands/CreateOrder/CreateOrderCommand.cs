using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AiCommerceApi.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    int UserId,
    string ShippingAddress
) : IRequest<CreateOrderResult>;