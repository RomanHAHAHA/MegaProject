using CartsService.Domain.Interfaces;
using Common.Domain.Dtos;
using MediatR;

namespace CartsService.Application.Features.CartItems.GetUserCart;

public class GetUserCartQueryHandler(
    ICartsRepository cartsRepository) : IRequestHandler<GetUserCartQuery, List<CartItemDto>>
{
    public async Task<List<CartItemDto>> Handle(
        GetUserCartQuery request, 
        CancellationToken cancellationToken)
    {
        var cartItems = await cartsRepository.GetUserCartByIdAsync(
            request.UserId, 
            cancellationToken);
        
        return cartItems
            .Select(ci => new CartItemDto
            {
                UserId = ci.UserId,
                Product = ci.ProductSnapshot!,
                Quantity = ci.Quantity
            }).ToList();
    }
}