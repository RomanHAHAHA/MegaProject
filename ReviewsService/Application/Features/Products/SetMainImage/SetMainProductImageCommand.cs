using MediatR;

namespace ReviewsService.Application.Features.Products.SetMainImage;

public record SetMainProductImageCommand(Guid ProductId, string ImagePath) : IRequest;