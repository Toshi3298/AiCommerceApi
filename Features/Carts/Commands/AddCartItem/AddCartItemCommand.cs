using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AiCommerceApi.Features.Carts.Commands.AddCartItem;

public record AddCartItemCommand(
    int UserId,
    int ProductId,
    int Quantity
) : IRequest<AddCartItemResult>;