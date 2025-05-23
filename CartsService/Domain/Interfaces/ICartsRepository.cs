using CartsService.Domain.Entities;

namespace CartsService.Domain.Interfaces;

public interface ICartsRepository
{
    Task<bool> CreateAsync(
        CartItem cartItem, 
        CancellationToken cancellationToken = default);
    
    Task<bool> DeleteAsync(
        CartItem cartItem, 
        CancellationToken cancellationToken = default);
    
    Task<bool> UpdateAsync(
        CartItem cartItem,
        CancellationToken cancellationToken = default);

    Task<CartItem?> GetByIdAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default);
    
    Task<List<CartItem>> GetUserCartByIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);

    Task<bool> DeleteItemsFromUserCartAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}