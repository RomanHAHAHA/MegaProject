using Common.Domain.Models.Results;
using MediatR;
using ReviewsService.Domain.Dtos;

namespace ReviewsService.Application.Features.Reviews.Update;

public record UpdateReviewCommand(ReviewCreateDto ReviewCreateDto) : IRequest<BaseResponse>;