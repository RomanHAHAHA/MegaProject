using MediatR;
using ReviewsService.Domain.Dtos;

namespace ReviewsService.Application.Features.Reviews.GetPendingReviews;

public record GetPendingReviewsQuery : IRequest<List<PendingReviewDto>>;