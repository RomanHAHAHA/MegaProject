using Common.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using ProductsService.Application.Common.Dtos;

namespace ProductsService.Application.Features.Categories.Commands.Update;

[ApiController]
[Route("api/categories")]
public class UpdateCategoryController(IMediator mediator) : ControllerBase
{
    [HttpPatch("{categoryId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategoryAsync(
        Guid categoryId,
        [FromBody] CategoryCreateDto categoryCreateDto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(User.GetId(), categoryId, categoryCreateDto);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}