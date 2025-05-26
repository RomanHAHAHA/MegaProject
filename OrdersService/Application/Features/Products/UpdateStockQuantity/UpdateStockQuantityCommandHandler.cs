using Common.Domain.Models.Results;
using MediatR;
using OrdersService.Domain.Interfaces;
using OrdersService.Infrastructure.Persistence;

namespace OrdersService.Application.Features.Products.UpdateStockQuantity;

public class UpdateStockQuantityCommandHandler(
    IProductRepository productRepository,
    OrdersDbContext dbContext) : IRequestHandler<UpdateStockQuantityCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(UpdateStockQuantityCommand request, CancellationToken cancellationToken)
    {
        var ids = request.CartItems.Select(p => p.Product.Id).ToList();
        var products = await productRepository.GetWithIdsAsync(ids, cancellationToken);
        
        if (products.Count != ids.Count)
        {
            return BaseResponse.NotFound("Some products not found");
        }
        
        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        foreach (var product in products)
        {
            var cartItem = request.CartItems.FirstOrDefault(ci => ci.Product.Id == product.Id);
            
            if (cartItem is null)
            {
                return BaseResponse.InternalServerError();
            }
            
            if (product.StockQuantity < cartItem.Quantity)
            {
                return BaseResponse.Conflict($"Not enough stock for product {product.Name}. There a only {product.StockQuantity}.");
            }

            product.StockQuantity -= cartItem.Quantity;
        }
        
        var saved = await productRepository.SaveChangesAsync(cancellationToken);

        if (!saved)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse.InternalServerError();
        }
        
        await transaction.CommitAsync(cancellationToken);
        return BaseResponse.Ok();
    }
}