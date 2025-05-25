using CartsService.Domain.Entities;

namespace CartsService.Domain.Interfaces;

public interface ICartsRepository
{
    Task CreateAsync(
        CartItem cartItem, 
        CancellationToken cancellationToken = default);
    
    void Delete(CartItem cartItem);
    
    void Update(CartItem cartItem);

    Task<CartItem?> GetByIdAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default);
    
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task<List<CartItem>> GetUserCartByIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default);
}