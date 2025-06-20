﻿using Common.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Application.Features.ProductImages.Create;
using ProductsService.Application.Features.ProductImages.Delete;
using ProductsService.Application.Features.ProductImages.SetMain;

namespace ProductsService.API.Controllers;

[ApiController]
[Route("/api/product-images")]
public class ProductImagesController(IMediator mediator) : ControllerBase
{
    [HttpPost("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> AddImagesAsync(
        [FromForm] List<IFormFile> images,
        Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new AddImagesCommand(images, productId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpDelete("{productImageId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteImageAsync(
        Guid productImageId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductImage(productImageId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpPatch("{imageId:guid}")]
    [Authorize]
    public async Task<IActionResult> SetMainImageAsync(
        Guid imageId,
        CancellationToken cancellationToken)
    {
        var command = new SetMainImageCommand(imageId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}