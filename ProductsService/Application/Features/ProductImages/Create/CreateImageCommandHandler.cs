using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.ProductImages.Create;

public class CreateImageCommandHandler(
    IProductsRepository productsRepository,
    IFileStorageService fileStorageService,
    IOptions<ProductImagesOptions> options,
    IPublishEndpoint publishEndpoint) : IRequestHandler<AddImagesCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        AddImagesCommand request, 
        CancellationToken cancellationToken)
    {
        var product = await productsRepository.GetByIdWithImagesAsync(
            request.ProductId, 
            cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(Product));
        }

        var images = new List<ProductImage>();

        for (var i = 0; i < request.Images.Count; i++)
        {
            var result = await fileStorageService.SaveFileAsync(
                request.Images[i], 
                options.Value.Path, 
                cancellationToken);

            if (result.IsFailure)
            {
                return BaseResponse.BadRequest(result.Error);
            }

            var isMain = i == 0 && product.Images.Count == 0;
            var image = new ProductImage
            {
                ProductId = product.Id,
                ImagePath = result.Value,
                IsMain = isMain
            };

            if (isMain)
            {
                await OnMainImageSet(image, cancellationToken);
            }

            images.Add(image);
        }
        
        product.Images.AddRange(images);
        var created = await productsRepository.UpdateAsync(product, cancellationToken);
        
        return created ? 
            BaseResponse.Ok() : 
            BaseResponse.InternalServerError("Failed to add image(s)");
    }

    private async Task OnMainImageSet(
        ProductImage image,
        CancellationToken cancellationToken = default)
    {
        var mainImageSetEvent = new ProductMainImageSetEvent(image.ProductId, image.ImagePath);
        await publishEndpoint.Publish(mainImageSetEvent, cancellationToken);
    }
}