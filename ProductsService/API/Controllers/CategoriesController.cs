using Common.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService.Application.Features.Categories.Common;
using ProductsService.Application.Features.Categories.Create;
using ProductsService.Application.Features.Categories.Delete;
using ProductsService.Application.Features.Categories.GetAll;
using ProductsService.Application.Features.Categories.Update;

namespace ProductsService.API.Controllers;

[Route("api/categories")]
[ApiController]
public class CategoriesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCategoryAsync(
        [FromBody] CategoryCreateDto categoryCreateDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(categoryCreateDto);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<List<ShortCategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetCategoriesQuery(), cancellationToken);
    }

    [HttpPatch("{categoryId:guid}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategoryAsync(
        Guid categoryId,
        [FromBody] CategoryCreateDto categoryCreateDto,
        CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(categoryId, categoryCreateDto);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [HttpDelete("{categoryId:guid}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(categoryId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}