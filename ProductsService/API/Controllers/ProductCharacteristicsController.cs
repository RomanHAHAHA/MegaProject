using Common.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Application.Features.ProductCharacteristics.Add;
using ProductsService.Application.Features.ProductCharacteristics.Remove;
using ProductsService.Application.Features.ProductCharacteristics.Update;
using ProductsService.Domain.Dtos;

namespace ProductsService.API.Controllers;

[Route("/api/product-characteristics")]
[ApiController]
public class ProductCharacteristicsController(IMediator mediator) : ControllerBase
{
    [HttpPost("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> AddCharacteristicsAsync(
        Guid productId,
        [FromBody] List<ProductCharacteristicDto> characteristics,
        CancellationToken cancellationToken)
    {
        var command = new AddCharacteristicsCommand(productId, characteristics);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpDelete("{productId:guid}/{name}")]
    [Authorize]
    public async Task<IActionResult> DeleteCharacteristicAsync(
        Guid productId,
        string name,
        CancellationToken cancellationToken)
    {
        var command = new RemoveCharacteristicCommand(productId, name);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpPatch("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductCharacteristicAsync(
        Guid productId,
        ProductCharacteristicUpdateDto updateDto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCharacteristicCommand(productId, updateDto);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}