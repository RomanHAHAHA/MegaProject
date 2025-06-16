using Common.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Application.Common.Dtos;

namespace ProductsService.Application.Features.Categories.Commands.Create;

[ApiController]
[Route("api/categories")]
public class CreateCategoryController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCategoryAsync(
        [FromBody] CategoryCreateDto categoryCreateDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(User.GetId(), categoryCreateDto);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}