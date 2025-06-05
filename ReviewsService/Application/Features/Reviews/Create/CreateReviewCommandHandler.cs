using Common.Application.Options;
using Common.Domain.Enums;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.SystemAction;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Application.Features.Reviews.Create;

public class CreateReviewCommandHandler(
    IReviewsRepository reviewsRepository,
    IUsersRepository usersRepository,
    IProductsRepository productsRepository,
    IPublishEndpoint publisher,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<CreateReviewCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var userExist = await usersRepository.ExistsAsync(request.UserId, cancellationToken);

        if (!userExist)
        {
            return BaseResponse.NotFound(nameof(UserSnapshot));
        }
        
        var productExist = await productsRepository
            .ExistsAsync(request.ReviewCreateDto.ProductId, cancellationToken);
        
        if (!productExist)
        {
            return BaseResponse.NotFound(nameof(ProductSnapshot));
        }
        
        var review = Review.FromCreateDto(request.ReviewCreateDto, request.UserId);
        
        await reviewsRepository.CreateAsync(review, cancellationToken);
        await OnReviewCreated(request, cancellationToken);
        
        var created = await reviewsRepository.SaveChangesAsync(cancellationToken);

        return created ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }

    private async Task OnReviewCreated(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        await publisher.Publish(new SystemActionEvent
        {
            CorrelationId = Guid.NewGuid(),
            SenderServiceName = serviceOptions.Value.Name,
            UserId = request.UserId,
            ActionType = ActionType.Create,
            Message = $"User {request.UserId} created review on product {request.ReviewCreateDto.ProductId}"
        }, cancellationToken);
    }
}