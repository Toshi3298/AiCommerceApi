using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderResult
    {
    public bool Success { get; set; }

    public string? Error { get; set; }

    public int? OrderId { get; set; }

    public decimal TotalPrice { get; set; }
    }
}