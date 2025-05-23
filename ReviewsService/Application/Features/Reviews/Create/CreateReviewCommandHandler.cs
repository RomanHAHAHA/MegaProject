using Common.Domain.Models.Results;
using MediatR;
using ReviewsService.Domain.Entities;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.Application.Features.Reviews.Create;

public class CreateReviewCommandHandler(
    IReviewsRepository reviewsRepository,
    IUsersRepository usersRepository,
    IProductsRepository productsRepository,
    ReviewsFactory reviewsFactory) : IRequestHandler<CreateReviewCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        CreateReviewCommand request, 
        CancellationToken cancellationToken)
    {
        var userExist = await usersRepository.ExistsAsync(
            request.ReviewCreateDto.UserId,
            cancellationToken);

        if (!userExist)
        {
            return BaseResponse.NotFound(nameof(UserSnapshot));
        }
        
        var productExist = await productsRepository.ExistsAsync(
            request.ReviewCreateDto.ProductId,
            cancellationToken);
        
        if (!productExist)
        {
            return BaseResponse.NotFound(nameof(ProductSnapshot));
        }
        
        var review = reviewsFactory.CreateFromDto(request.ReviewCreateDto);
        
        await reviewsRepository.CreateAsync(review, cancellationToken);

        return BaseResponse.Ok();
    }
}