using CartsService.Application.Features.CartItems.Create;
using CartsService.Application.Features.CartItems.Decrement;
using CartsService.Application.Features.CartItems.Delete;
using CartsService.Application.Features.CartItems.GetUserCart;
using CartsService.Application.Features.CartItems.Increment;
using Common.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartsService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/carts")]
public class CartsController(IMediator mediator) : ControllerBase
{
    [HttpPost("{productId:guid}")]
    public async Task<IActionResult> AddProductToCartAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new AddProductToCartCommand(User.GetId(), productId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetUserCartAsync(CancellationToken cancellationToken)
    {
        var query = new GetUserCartQuery(User.GetId());
        var cartItems = await mediator.Send(query, cancellationToken);
        return Ok(new { data = cartItems });
    }
    
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserCartAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserCartQuery(userId);
        var cartItems = await mediator.Send(query, cancellationToken);
        return Ok(new { data = cartItems });
    }

    [HttpPatch("{productId:guid}/increment")]
    public async Task<IActionResult> IncrementProductCountAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new IncrementItemQuantityCommand(User.GetId(), productId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
    
    [HttpPatch("{productId:guid}/decrement")]
    public async Task<IActionResult> DecrementProductCountAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new DecrementItemQuantityCommand(User.GetId(), productId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
    
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeleteItemFromCartAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteItemCommand(User.GetId(), productId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}