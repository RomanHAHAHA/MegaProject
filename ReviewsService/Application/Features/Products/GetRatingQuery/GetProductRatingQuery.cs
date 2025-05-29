using Common.Domain.Models.Results;
using MediatR;

namespace ReviewsService.Application.Features.Products.GetRatingQuery;

public record GetProductRatingQuery(Guid ProductId) : IRequest<double>;