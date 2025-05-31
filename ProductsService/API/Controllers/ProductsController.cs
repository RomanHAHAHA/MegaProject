using Common.API.Extensions;
using Common.Domain.Dtos;
using Common.Domain.Models.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Application.Features.Products.AddCategory;
using ProductsService.Application.Features.Products.Common;
using ProductsService.Application.Features.Products.Create;
using ProductsService.Application.Features.Products.Delete;
using ProductsService.Application.Features.Products.GetPagedList;
using ProductsService.Application.Features.Products.GetProductInfo;
using ProductsService.Application.Features.Products.GetQuantity;
using ProductsService.Application.Features.Products.Update;

namespace ProductsService.API.Controllers;

[Route("api/products")]
[ApiController]
public class ProductsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProductAsync(
        [FromBody] ProductCreateDto productCreateDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(productCreateDto, User.GetId());
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<PagedList<ProductPagedDto>> GetProductsAsync(
        [FromQuery] ProductFilter productFilter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams,
        CancellationToken cancellationToken)
    {
        var userId = User.GetId();

        if (userId != Guid.Empty)
        {
            productFilter.UserId = userId;
        }
        
        var command = new GetProductsQuery(productFilter, sortParams, pageParams);
        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("{productId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductInfoAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var query = new GetProductInfoQuery(productId);
        var response = await mediator.Send(query, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpPut("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductAsync(
        Guid productId,
        [FromBody] ProductCreateDto productCreateDto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(productId, productCreateDto);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpDelete("{productId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(productId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpPost("{productId:guid}/categories/{categoryId:guid}")]
    [Authorize]
    public async Task<IActionResult> AddCategoryAsync(
        Guid productId,
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var command = new AddCategoryCommand(productId, categoryId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpGet("{productId:guid}/quantity")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductQuantityAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new GetQuantityQuery(productId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}