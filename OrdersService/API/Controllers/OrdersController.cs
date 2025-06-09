using Common.API.Extensions;
using Common.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Application.Features.HasUserOrderedProduct;
using OrdersService.Application.Features.Orders.Create;
using OrdersService.Application.Features.Orders.GetConfirmedOrders;
using OrdersService.Application.Features.Orders.GetUsersOrders;
using OrdersService.Application.Features.Orders.SetStatus;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Interfaces;

namespace OrdersService.API.Controllers;

[Route("/api/orders")]
[ApiController]
public class OrdersController(
    ICartServiceClient cartServiceClient,
    IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync(
        DeliveryLocationCreateDto deliveryLocationCreateDto,
        CancellationToken cancellationToken)
    {
        var userId = User.GetId();
        var cartItems = await cartServiceClient.GetCartItemsAsync(userId, cancellationToken);
        
        var command = new CreateOrderCommand(userId, deliveryLocationCreateDto, cartItems.ToList());
        var response = await mediator.Send(command, cancellationToken);
        
        return this.HandleResponse(response);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrdersAsync(CancellationToken cancellationToken)
    {
        var command = new GetUserOrdersCommand(User.GetId());
        var orders = await mediator.Send(command, cancellationToken);
        return Ok(new { data = orders });
    }
    
    [Authorize]
    [HttpGet("confirmed")]
    public async Task<IActionResult> GetConfirmedOrdersAsync(CancellationToken cancellationToken)
    {
        var command = new GetConfirmedOrdersQuery();
        var orders = await mediator.Send(command, cancellationToken);
        return Ok(new { data = orders });
    }

    [HttpPatch("{orderId:guid}/{status}")]
    [Authorize]
    public async Task<IActionResult> SetOrderStatusAsync(
        Guid orderId,
        OrderStatus status,
        CancellationToken cancellationToken)
    {
        var command = new SetOrderStatusCommand(orderId, status);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [Authorize]
    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> HasUserOrderedProductAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new HasUserOrderedProductQuery(
            User.GetId(),
            productId);
        
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}