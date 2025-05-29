using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.ProductCharacteristics.Add;

public class AddCharacteristicsCommandHandler(
    IProductsRepository productsRepository) : IRequestHandler<AddCharacteristicsCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(AddCharacteristicsCommand request, CancellationToken cancellationToken)
    {
        var product = await productsRepository
            .GetByIdWithCharacteristicsAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(Product));
        }
        
        var characteristics = request.Characteristics
            .Select(c => new ProductCharacteristic
            {
                ProductId = request.ProductId,
                Name = c.Name,
                Value = c.Value
            }).ToList();

        foreach (var characteristic in characteristics
                .Where(characteristic => product.Characteristics
                    .Any(c => c.Name == characteristic.Name)))
        {
            return BaseResponse.Conflict($"Characteristic \"{characteristic.Name}\" already exists");
        }
        
        product.Characteristics.AddRange(characteristics);
        var added = await productsRepository.SaveChangesAsync(cancellationToken);

        return added ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }
}