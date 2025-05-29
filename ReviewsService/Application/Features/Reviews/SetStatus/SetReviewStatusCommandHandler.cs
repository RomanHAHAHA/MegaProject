using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Application.Features.Reviews.SetStatus;

public class SetReviewStatusCommandHandler(
    IReviewsRepository reviewsRepository,
    IPublishEndpoint publisher,
    IHttpUserContext httpContext) : IRequestHandler<SetReviewStatusCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(SetReviewStatusCommand request, CancellationToken cancellationToken)
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
        await OnReviewStatusSet(request, cancellationToken);
        
        var updated = await reviewsRepository.SaveChangesAsync(cancellationToken);
        
        return updated ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }

    private async Task OnReviewStatusSet(SetReviewStatusCommand request, CancellationToken cancellationToken)
    {
        var reviewUserId = request.UserId;
        var reviewProductId = request.ProductId;
        
        await publisher.Publish(new SystemActionEvent
        {
            UserId = httpContext.UserId,
            ActionType = ActionType.Update,
            Message = $"Review of user {reviewUserId} on product {reviewProductId} status set: {request.Status}",
        }, cancellationToken);
        
        await publisher.Publish(
            new ReviewStatusUpdatedEvent(reviewProductId), 
            cancellationToken);
    }
}