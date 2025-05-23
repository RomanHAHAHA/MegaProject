using Microsoft.EntityFrameworkCore;
using ReviewsService.Domain.Dtos;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Enums;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Infrastructure.Persistence.Repositories.Base;

public class ReviewsRepository(ReviewsDbContext dbContext) : IReviewsRepository
{
    public async Task<bool> CreateAsync(
        Review review,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Reviews.AddAsync(review, cancellationToken);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(
        Review review,
        CancellationToken cancellationToken = default)
    {
        dbContext.Reviews.Update(review);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<Review?> GetByIdAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .FirstOrDefaultAsync(r => 
                r.ProductId == productId && r.UserId == userId, 
                cancellationToken);
    }

    public async Task<bool> ExistAsync(
        Guid userId,
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .AnyAsync(r => 
                r.ProductId == productId && r.UserId == userId,
                cancellationToken);
    }

    public async Task<bool> DeleteAsync(
        Review review,
        CancellationToken cancellationToken = default)
    {
        dbContext.Reviews.Remove(review);
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<List<ProductReviewDto>> GetProductReviewsAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .AsNoTracking()
            .AsSplitQuery()
            .Include(r => r.User)
            .Include(r => r.Votes)
            .Where(r => r.ProductId == productId && r.Status == ReviewStatus.Approved)
            .Select(r => new ProductReviewDto
            {
                UserId = r.UserId,
                ProductId = r.ProductId,
                NickName = r.User!.NickName,
                AvatarPath = r.User!.AvatarPath,
                Text = r.Text,
                Rate = r.Rate,
                CreatedAt = $"{r.CreatedAt.ToLocalTime():dd.MM.yyyy HH:mm}",
                LikesCount = r.Votes.Count(v => v.VoteType == VoteType.Like),
                DislikesCount = r.Votes.Count(v => v.VoteType == VoteType.Dislike)
            })
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<PendingReviewDto>> GetPendingReviewsAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Reviews
            .AsNoTracking()
            .AsSplitQuery()
            .Include(r => r.User)
            .Include(r => r.Votes)
            .Where(r => r.Status == ReviewStatus.Pending)
            .Select(r => new PendingReviewDto()
            {
                UserId = r.UserId,
                ProductId = r.ProductId,
                NickName = r.User!.NickName,
                AvatarPath = r.User!.AvatarPath,
                Text = r.Text,
                Rate = r.Rate,
                CreatedAt = $"{r.CreatedAt.ToLocalTime():dd.MM.yyyy HH:mm}",
            })
            .ToListAsync(cancellationToken);
    }
}