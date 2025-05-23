using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;
using ProductsService.Infrastructure.Persistence;

namespace ProductsService.Application.Features.ProductImages.SetMain;

public class SetMainImageCommandHandler(
    IProductImagesRepository imagesRepository,
    ProductsDbContext dbContext,
    IPublishEndpoint publishEndpoint) : IRequestHandler<SetMainImageCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        SetMainImageCommand request, 
        CancellationToken cancellationToken)
    {
        var newMainImage = await imagesRepository.GetByIdAsync(request.ImageId, cancellationToken);

        if (newMainImage is null)
        {
            return BaseResponse.NotFound("Image not found");
        }
        
        newMainImage.IsMain = true;
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var newMainImageUpdated = await imagesRepository.UpdateAsync(newMainImage, cancellationToken);

        if (!newMainImageUpdated)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse.InternalServerError("Failed to set new main image");
        }
        
        var productImages = await imagesRepository
            .GetProductImagesAsync(newMainImage.ProductId, cancellationToken);

        var pastMainImage = productImages.FirstOrDefault(i => i.IsMain && i.Id != request.ImageId);

        if (pastMainImage is null)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse.NotFound("Failed to find past main image");
        }
        
        pastMainImage.IsMain = false;
        var updatedPastMainImage = await imagesRepository.UpdateAsync(pastMainImage, cancellationToken);

        if (!updatedPastMainImage)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse.InternalServerError("Failed to update past main image");
        }
        
        await transaction.CommitAsync(cancellationToken);
        await OnMainImageSet(newMainImage, cancellationToken);
            
        return BaseResponse.Ok();
    }

    
    private async Task OnMainImageSet(
        ProductImage image,
        CancellationToken cancellationToken = default)
    {
        var mainImageSetEvent = new ProductMainImageSetEvent(image.ProductId, image.ImagePath);
        await publishEndpoint.Publish(mainImageSetEvent, cancellationToken);
    }
}