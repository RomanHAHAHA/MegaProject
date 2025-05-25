using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;
using ProductsService.Infrastructure.Persistence;

namespace ProductsService.Application.Features.ProductImages.Delete;

public class DeleteProductImageHandler(
    IProductImagesRepository imagesRepository,
    IFileStorageService fileStorageService,
    IPublishEndpoint publishEndpoint,
    ProductsDbContext dbContext) : IRequestHandler<DeleteProductImage, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        DeleteProductImage request, 
        CancellationToken cancellationToken)
    {
        var image = await imagesRepository.GetByIdAsync(request.ImageId, cancellationToken);

        if (image is null)
        {
            return BaseResponse.NotFound("Product image");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        imagesRepository.Delete(image);
        await imagesRepository.SaveChangesAsync(cancellationToken);

        var deleteFileResult = await fileStorageService.DeleteFileAsync(
            image.ImagePath,
            cancellationToken);

        if (deleteFileResult.IsFailure)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse.InternalServerError("Failed to delete image from file system");
        }

        if (image.IsMain)
        {
            await SetNewMainImageAsync(image.ProductId, cancellationToken);
        }
        
        await imagesRepository.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        
        return BaseResponse.Ok();
    }
    
    private async Task SetNewMainImageAsync(
        Guid productId, 
        CancellationToken cancellationToken)
    {
        var images = await imagesRepository.GetProductImagesAsync(productId, cancellationToken);
        var newMainImage = images.FirstOrDefault();

        if (newMainImage is null)
        {
            return; 
        }

        newMainImage.IsMain = true;
        
        await OnMainImageSet(newMainImage, cancellationToken);
    }
    
    private async Task OnMainImageSet(
        ProductImage image,
        CancellationToken cancellationToken)
    {
        var mainImageSetEvent = new ProductMainImageSetEvent(image.ProductId, image.ImagePath);
        await publishEndpoint.Publish(mainImageSetEvent, cancellationToken);
    }
}