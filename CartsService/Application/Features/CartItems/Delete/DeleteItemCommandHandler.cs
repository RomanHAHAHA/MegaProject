﻿using CartsService.Domain.Entities;
using CartsService.Domain.Interfaces;
using Common.Domain.Models.Results;
using MediatR;

namespace CartsService.Application.Features.CartItems.Delete;

public class DeleteItemCommandHandler(ICartsRepository cartsRepository) : 
    IRequestHandler<DeleteItemCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await cartsRepository.GetByIdAsync(
            request.UserId,
            request.ProductId,
            cancellationToken);

        if (cartItem is null)
        {
            return BaseResponse.NotFound(nameof(CartItem));
        }

        cartsRepository.Delete(cartItem);
        var removed = await cartsRepository.SaveChangesAsync(cancellationToken);
        
        return removed ?
            BaseResponse.Ok() : 
            BaseResponse.InternalServerError("Failed to remove item");
    }
}