using Common.Domain.Models.Results;
using MediatR;
using ReviewsService.Domain.Dtos;

namespace ReviewsService.Application.Features.Reviews.Create;

public record CreateReviewCommand(ReviewCreateDto ReviewCreateDto) : IRequest<BaseResponse>;