using ReviewsService.Domain.Dtos;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Enums;

namespace ReviewsService.Application.Features.Reviews.Create;

public class ReviewsFactory
{
    public Review CreateFromDto(ReviewCreateDto reviewCreateDto, Guid userId)
    {
        return new Review
        {
            UserId = userId,
            ProductId = reviewCreateDto.ProductId,
            Text = reviewCreateDto.Text,
            Rate = reviewCreateDto.Rate,
            Status = ReviewStatus.Pending,
            CreatedAt = DateTime.UtcNow,
        };
    }
}