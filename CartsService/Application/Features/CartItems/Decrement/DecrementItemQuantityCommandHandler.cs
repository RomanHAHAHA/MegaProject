﻿using CartsService.Domain.Entities;
using CartsService.Domain.Interfaces;
using Common.Domain.Models.Results;
using MediatR;

namespace CartsService.Application.Features.CartItems.Decrement;

public class DecrementItemQuantityCommandHandler(
    ICartsRepository cartsRepository) : IRequestHandler<DecrementItemQuantityCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        DecrementItemQuantityCommand request, 
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
        
        //TODO: logic if quantity is 0
        cartItem.Quantity--;
        var updated = await cartsRepository.SaveChangesAsync(cancellationToken);
        
        return updated ? 
            BaseResponse.Ok() :
            BaseResponse.InternalServerError("Failed to decrement item quantity");
    }
}