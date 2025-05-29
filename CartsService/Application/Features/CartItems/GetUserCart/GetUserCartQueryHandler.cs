using CartsService.Domain.Interfaces;
using Common.Domain.Dtos;
using MediatR;

namespace CartsService.Application.Features.CartItems.GetUserCart;

public class GetUserCartQueryHandler(
    ICartsRepository cartsRepository) : IRequestHandler<GetUserCartQuery, CartDto>
{
    public async Task<CartDto> Handle(
        GetUserCartQuery request, 
        CancellationToken cancellationToken)
    {
        var cartItems = await cartsRepository.GetUserCartByIdAsync(
            request.UserId, 
            cancellationToken);

        return new CartDto
        {
            UserId = request.UserId,
            CartItems = cartItems
                .Select(ci => new CartItemDto
                {
                    Product = ci.ProductSnapshot!,
                    Quantity = ci.Quantity
                }).ToList(),
        };;
    }
}

public class CartDto
{
    public Guid UserId { get; set; }

    public List<CartItemDto> CartItems { get; set; } = [];

    public decimal TotalCartPrice => CartItems.Sum(ci => ci.TotalPrice);
}