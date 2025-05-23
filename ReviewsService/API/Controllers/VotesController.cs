using Common.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewsService.Application.Features.Votes.Set;
using ReviewsService.Domain.Enums;

namespace ReviewsService.API.Controllers;

[Route("/api/review-votes")]
[ApiController]
public class VotesController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SetReviewVoteAsync(
        Guid userId,
        Guid productId,
        VoteType voteType,
        CancellationToken cancellationToken)
    {
        var command = new SetReviewVoteCommand(User.GetId(), userId, productId, voteType);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}