using Common.Domain.Models.Results;
using MediatR;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Application.Features.Reviews.SetStatus;

public class SetReviewStatusCommandHandler(
    IReviewsRepository reviewsRepository) : IRequestHandler<SetReviewStatusCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        SetReviewStatusCommand request, 
        CancellationToken cancellationToken)
    {
        var review = await reviewsRepository.GetByIdAsync(
            request.UserId,
            request.ProductId,
            cancellationToken);

        if (review is null)
        {
            return BaseResponse.NotFound(nameof(Review));
        }
        
        review.Status = request.Status;
        
        await reviewsRepository.UpdateAsync(review, cancellationToken);
        
        return BaseResponse.Ok();
    }
}