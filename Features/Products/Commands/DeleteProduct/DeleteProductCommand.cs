using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace AiCommerceApi.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id)
    : IRequest<DeleteProductResult>;