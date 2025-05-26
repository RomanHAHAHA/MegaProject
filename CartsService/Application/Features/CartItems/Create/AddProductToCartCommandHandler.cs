using CartsService.Domain.Entities;
using CartsService.Domain.Interfaces;
using Common.Domain.Entities;
using Common.Domain.Models.Results;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CartsService.Application.Features.CartItems.Create;

public class AddProductToCartCommandHandler(
    ICartsRepository cartsRepository,
    IProductRepository productRepository) : IRequestHandler<AddProductToCartCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(AddProductToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse.NotFound(nameof(ProductSnapshot));
        }
        
        var cartItem = await cartsRepository.GetByIdAsync(
            request.UserId,
            request.ProductId,
            cancellationToken);

        if (cartItem is not null)
        {
            return BaseResponse.Conflict("Product is already in cart exists");
        }
        
        cartItem = new CartItem
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            Quantity = 1,
        };

        await cartsRepository.CreateAsync(cartItem, cancellationToken);
        var created = await cartsRepository.SaveChangesAsync(cancellationToken);

        if (!created)
        {
            return BaseResponse.InternalServerError();
        }
        
        var stockQuantity = await productRepository
            .GetStockQuantityById(request.ProductId, cancellationToken);

        return stockQuantity switch
        {
            null => BaseResponse.NotFound("Product"),
            0 => BaseResponse.Conflict("Stock quantity is zero"),
            <= 5 => BaseResponse.Ok("Warning: only few items left on stock"),
            _ => BaseResponse.Ok()
        }; 
    }
}