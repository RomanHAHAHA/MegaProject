using Common.Domain.Entities;
using Common.Domain.Models.Results;
using MediatR;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.HasUserOrderedProduct;

public class HasUserOrderedProductQueryHandler(
    IOrdersRepository ordersRepository,
    IUsersRepository usersRepository,
    IProductRepository productRepository) : 
    IRequestHandler<HasUserOrderedProductQuery, BaseResponse<bool>>
{
    public async Task<BaseResponse<bool>> Handle(
        HasUserOrderedProductQuery request, 
        CancellationToken cancellationToken)
    {
        var user = await usersRepository
            .GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return BaseResponse<bool>.NotFound(nameof(UserSnapshot));
        }
        
        var product = await productRepository
            .GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return BaseResponse<bool>.NotFound(nameof(ProductSnapshot));
        }
        
        return await ordersRepository.HasUserOrderedProductAsync(
            request.UserId,
            request.ProductId,
            cancellationToken);
    }
}