using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AiCommerceApi.Dtos.Carts;

namespace AiCommerceApi.Features.Carts.Queries.GetCart;

public class GetCartResult
{
    public bool Success { get; set; }

    public string? Error { get; set; }

    public CartResponseDto? Cart { get; set; }
}