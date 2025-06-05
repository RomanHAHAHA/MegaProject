using System.Data;
using Common.Application.Options;
using Common.Domain.Dtos;
using Common.Infrastructure.Messaging.Events.Product;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;
using ProductsService.Infrastructure.Persistence;

namespace ProductsService.Application.Features.Products.Reserve;

public class ReserveOrderProductsCommandHandler(
    IProductsRepository productsRepository,
    IPublishEndpoint publisher,
    ProductsDbContext dbContext,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<ReserveOrderProductsCommand>
{
    public async Task Handle(ReserveOrderProductsCommand request, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var connection = dbContext.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var transaction = await connection.BeginTransactionAsync(
                IsolationLevel.Serializable, cancellationToken);

            try
            {
                await dbContext.Database.UseTransactionAsync(transaction, cancellationToken);

                var productIds = request.CartItems.Select(ci => ci.Product.Id).ToList();
                var products = await productsRepository.GetProductsByIdsAsync(productIds, cancellationToken);

                if (products.Count != request.CartItems.Count)
                {
                    await PublishProductsReservationFailedEvent(request, [], cancellationToken);
                    await transaction.RollbackAsync(cancellationToken);
                    return;
                }

                var outOfStock = GetOutOfStockProducts(products, request.CartItems);

                if (outOfStock.Count != 0)
                {
                    await PublishProductsReservationFailedEvent(request, outOfStock, cancellationToken);
                    await transaction.RollbackAsync(cancellationToken);
                    return;
                }

                UpdateStock(products, request.CartItems);
                await dbContext.SaveChangesAsync(cancellationToken);

                await PublishProductsReservedEvent(request, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
    
    private void UpdateStock(List<Product> products, List<CartItemDto> cartItems)
    {
        var requestedQuantities = cartItems.ToDictionary(ci => ci.Product.Id, ci => ci.Quantity);

        foreach (var product in products)
        {
            if (requestedQuantities.TryGetValue(product.Id, out var requestedQuantity))
            {
                product.StockQuantity -= requestedQuantity;
            }
        }
    }

    private List<ProductStockInfo> GetOutOfStockProducts(List<Product> products, List<CartItemDto> cartItems)
    {
        var outOfStock = new List<ProductStockInfo>();
        var requestedQuantities = cartItems.ToDictionary(ci => ci.Product.Id, ci => ci.Quantity);

        foreach (var product in products)
        {
            if (!requestedQuantities.TryGetValue(product.Id, out var requestedQuantity) ||
                product.StockQuantity < requestedQuantity)
            {
                outOfStock.Add(new ProductStockInfo(product.Id, product.StockQuantity));
            }
        }

        return outOfStock;
    }
    
    private async Task PublishProductsReservationFailedEvent(
        ReserveOrderProductsCommand request,
        List<ProductStockInfo> productStockInfos,
        CancellationToken cancellationToken)
    {
        await publisher.Publish(
            new ProductsReservationFailedEvent
            {
                CorrelationId = Guid.NewGuid(),
                SenderServiceName = serviceOptions.Value.Name,
                OrderId = request.OrderId,
                UserId = request.UserId,
                ProductStockInfos = productStockInfos
            }, 
            cancellationToken);
    }

    private async Task PublishProductsReservedEvent(
        ReserveOrderProductsCommand request,
        CancellationToken cancellationToken)
    {
        await publisher.Publish(
            new ProductsReservedEvent
            {
                CorrelationId = Guid.NewGuid(),
                SenderServiceName = serviceOptions.Value.Name,
                OrderId = request.OrderId,
                UserId = request.UserId,
            }, 
            cancellationToken);
    }
}