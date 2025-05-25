using CartsService.Domain.Entities;
using CartsService.Domain.Interfaces;
using Common.Domain.Enums;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MediatR;

namespace CartsService.Application.Features.CartItems.Increment;

public class IncrementItemQuantityCommandHandler(
    ICartsRepository cartsRepository) : IRequestHandler<IncrementItemQuantityCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        IncrementItemQuantityCommand request, 
        CancellationToken cancellationToken)
    {
        var cartItem = await cartsRepository.GetByIdAsync(
            request.UserId,
            request.ProductId,
            cancellationToken);

        if (cartItem is null)
        {
            return BaseResponse.NotFound(nameof(CartItem));
        }
        
        cartItem.Quantity++;
        var updated = await cartsRepository.SaveChangesAsync(cancellationToken);
        
        return updated ? 
            BaseResponse.Ok() :
            BaseResponse.InternalServerError("Failed to increment item quantity");
    }
}