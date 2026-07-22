using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AiCommerceApi.Models
{
    public enum OrderStatus
{
    Pending = 1,
    Preparing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5
}
}
