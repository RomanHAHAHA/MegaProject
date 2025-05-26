using Common.Domain.Enums;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ReviewsService.Application.Features.Reviews.Create;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Application.Features.Reviews.Update;

public class UpdateReviewCommandHandler(
    IReviewsRepository reviewsRepository,
    IPublishEndpoint publisher) : IRequestHandler<UpdateReviewCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await reviewsRepository.GetByIdAsync(
            request.UserId,
            request.ReviewCreateDto.ProductId,
            cancellationToken);

        if (review is null)
        {
            return BaseResponse.NotFound(nameof(Review));
        }
        
        UpdateReview(review, request.ReviewCreateDto);
        await OnReviewUpdated(request, cancellationToken);
        
        var updated = await reviewsRepository.SaveChangesAsync(cancellationToken);

        return updated ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }

    private async Task OnReviewUpdated(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var productId = request.ReviewCreateDto.ProductId;

        await publisher.Publish(new SystemActionEvent
        {
            UserId = request.UserId,
            ActionType = ActionType.Update,
            Message = $"Review of user {userId} on product {productId} updated"
        }, cancellationToken);
    }
    
    private void UpdateReview(Review review, ReviewCreateDto reviewCreateDto)
    {
        review.Text = reviewCreateDto.Text;
        review.Rate = reviewCreateDto.Rate;
    }
}