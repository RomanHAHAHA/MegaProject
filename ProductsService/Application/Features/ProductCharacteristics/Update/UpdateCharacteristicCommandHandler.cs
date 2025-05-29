using Common.Domain.Models.Results;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.ProductCharacteristics.Update;

public class UpdateCharacteristicCommandHandler(
    IProductCharacteristicsRepository characteristicsRepository) : 
    IRequestHandler<UpdateCharacteristicCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(UpdateCharacteristicCommand request, CancellationToken cancellationToken)
    {
        var characteristic = await characteristicsRepository.GetByIdAsync(
            request.ProductId,
            request.CharacteristicDto.OldName,
            cancellationToken);

        if (characteristic is null)
        {
            return BaseResponse.NotFound(nameof(ProductCharacteristic));
        }
        
        var characteristicsNames = (await characteristicsRepository
                .GetProductCharacteristicsAsync(request.ProductId, cancellationToken))
                    .Select(pc => pc.Name)
                    .ToList();
                
        if (characteristicsNames.Contains(request.CharacteristicDto.NewName))
        {
            return BaseResponse.Conflict($"Characteristic \"{request.CharacteristicDto.NewName}\" already exists.");
        }
        
        characteristicsRepository.Delete(characteristic);

        var updatedCharacteristic = new ProductCharacteristic
        {
            ProductId = request.ProductId,
            Name = request.CharacteristicDto.NewName,
            Value = request.CharacteristicDto.Value
        };
        
        await characteristicsRepository.CreateAsync(updatedCharacteristic, cancellationToken);
        
        var updated = await characteristicsRepository.SaveChangesAsync(cancellationToken);

        return updated ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }
}