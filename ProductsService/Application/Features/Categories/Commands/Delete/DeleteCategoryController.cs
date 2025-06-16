using Common.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace ProductsService.Application.Features.Categories.Commands.Delete;

[ApiController]
[Route("api/categories")]
public class DeleteCategoryController(IMediator mediator) : ControllerBase
{
    [HttpDelete("{categoryId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(User.GetId(), categoryId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}