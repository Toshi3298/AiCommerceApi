    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MediatR;

    namespace AiCommerceApi.Features.Products.Commands.UpdateProduct;

    public record UpdateProductCommand(
        int Id,
        string Name,
        string Description,
        string Brand,
        decimal Price,
        int Stock,
        int CategoryId,
        bool IsActive,
        string? ImageUrl
    ) : IRequest<UpdateProductResult>;