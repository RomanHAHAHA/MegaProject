using MediatR;

namespace CartsService.Application.Features.Products.SetMainImage;

public record SetMainProductImageCommand(Guid ProductId, string ImagePath) : IRequest;