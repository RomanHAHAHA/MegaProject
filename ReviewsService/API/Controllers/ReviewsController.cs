using Common.API.Authentication;
using Common.API.Extensions;
using Common.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewsService.Application.Features.Products.GetRatingQuery;
using ReviewsService.Application.Features.Reviews.Create;
using ReviewsService.Application.Features.Reviews.GetPendingReviews;
using ReviewsService.Application.Features.Reviews.GetProductReviews;
using ReviewsService.Application.Features.Reviews.SetStatus;
using ReviewsService.Application.Features.Reviews.Update;
using ReviewsService.Domain.Enums;
using ReviewsService.Domain.Interfaces;

namespace ReviewsService.API.Controllers;

[Route("/api/reviews")]
[ApiController]
public class ReviewsController(
    IOrderServiceClient orderServiceClient,
    IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateReviewAsync(
        ReviewCreateDto reviewCreateDto,
        CancellationToken cancellationToken)
    {
        var orderServiceResponse = await orderServiceClient.HasUserOrderedProductAsync(
            reviewCreateDto.ProductId, 
            cancellationToken);

        if (!orderServiceResponse.Data)
        {
            return Conflict(new { descriprion = "You have not ordered this product" });
        }
            
        var command = new CreateReviewCommand(reviewCreateDto, User.GetId());
        var response = await mediator.Send(command, cancellationToken);
        
        return this.HandleResponse(response);
    }

    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> UpdateReviewAsync(
        ReviewCreateDto reviewCreateDto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateReviewCommand(reviewCreateDto, User.GetId());
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpPatch("status/{status}")]
    [Authorize]
    public async Task<IActionResult> SetReviewStatusAsync(
        Guid userId,
        Guid productId,
        ReviewStatus status,
        CancellationToken cancellationToken)
    {
        var command = new SetReviewStatusCommand(userId, productId, status);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpGet("product/{productId:guid}")]
    [AllowAnonymous]
    public async Task<List<ProductReviewDto>> GetProductReviewsAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var query = new GetProductReviewsQuery(productId);
        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("product/{productId:guid}/rating")]
    public async Task<IActionResult> GetProductRatingAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        var query = new GetProductRatingQuery(productId);
        var rating = await mediator.Send(query, cancellationToken);
        return Ok(new { data = rating });
    }

    [HttpGet("pending")]
    [Authorize]
    public async Task<List<PendingReviewDto>> GetPendingReviewsAsync(
        CancellationToken cancellationToken)
    {
        var query = new GetPendingReviewsQuery();
        return await mediator.Send(query, cancellationToken);
    }
}