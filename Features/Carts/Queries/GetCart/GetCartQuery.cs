using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AiCommerceApi.Features.Carts.Queries.GetCart;

public record GetCartQuery(
    int UserId
) : IRequest<GetCartResult>;