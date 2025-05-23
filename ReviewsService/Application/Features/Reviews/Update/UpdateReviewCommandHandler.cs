using Common.Domain.Models.Results;
using MediatR;
using ReviewsService.Domain.Dtos;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Application.Features.Reviews.Update;

public class UpdateReviewCommandHandler(
    IReviewsRepository reviewsRepository) : IRequestHandler<UpdateReviewCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        UpdateReviewCommand request, 
        CancellationToken cancellationToken)
    {
        var review = await reviewsRepository.GetByIdAsync(
            request.ReviewCreateDto.UserId,
            request.ReviewCreateDto.ProductId,
            cancellationToken);

        if (review is null)
        {
            return BaseResponse.NotFound(nameof(Review));
        }
        
        UpdateReview(review, request.ReviewCreateDto);
        
        await reviewsRepository.UpdateAsync(review, cancellationToken);

        return BaseResponse.Ok();
    }

    private void UpdateReview(Review review, ReviewCreateDto reviewCreateDto)
    {
        review.Text = reviewCreateDto.Text;
        review.Rate = reviewCreateDto.Rate;
    }
}