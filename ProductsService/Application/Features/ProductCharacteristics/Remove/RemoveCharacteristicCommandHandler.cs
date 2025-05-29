using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.ProductCharacteristics.Remove;

public class RemoveCharacteristicCommandHandler(
    IProductCharacteristicsRepository characteristicsRepository) : 
    IRequestHandler<RemoveCharacteristicCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        RemoveCharacteristicCommand request, 
        CancellationToken cancellationToken)
    {
        var characteristic = await characteristicsRepository.GetByIdAsync(
            request.ProductId,
            request.Name,
            cancellationToken);

        if (characteristic is null)
        {
            return BaseResponse.NotFound(nameof(ProductCharacteristic));
        }
        
        characteristicsRepository.Delete(characteristic);
        
        var deleted = await characteristicsRepository.SaveChangesAsync(cancellationToken);

        return deleted ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }
}