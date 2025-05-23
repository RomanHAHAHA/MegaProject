using CartsService.Domain.Entities;
using CartsService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartsService.Infrastructure.Persistence.Repositories.Base;

public class CartsRepository(CartsDbContext dbContext) : ICartsRepository
{
    public async Task<bool> CreateAsync(
        CartItem cartItem, 
        CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(cartItem, cancellationToken);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(
        CartItem cartItem, 
        CancellationToken cancellationToken = default)
    {
        dbContext.CartItems.Remove(cartItem);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(
        CartItem cartItem, 
        CancellationToken cancellationToken = default)
    {
        dbContext.CartItems.Update(cartItem);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0; 
    }

    public async Task<CartItem?> GetByIdAsync(
        Guid userId, 
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CartItems
            .FirstOrDefaultAsync(ci => 
                ci.UserId == userId && ci.ProductId == productId, 
                cancellationToken);
    }

    public async Task<List<CartItem>> GetUserCartByIdAsync(
        Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await dbContext.CartItems
            .AsNoTracking()
            .Include(ci => ci.ProductSnapshot)
            .Where(ci => ci.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> DeleteItemsFromUserCartAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var cartItems = await dbContext.CartItems
            .Where(ci => ci.UserId == userId)
            .ToListAsync(cancellationToken);
        
        dbContext.RemoveRange(cartItems);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}