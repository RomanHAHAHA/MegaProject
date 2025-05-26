using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Extensions;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Categories.Update;

public class UpdateCategoryCommandHandler(
    ICategoriesRepository categoriesRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpContext) : IRequestHandler<UpdateCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoriesRepository.GetByIdAsync(
            request.CategoryId, 
            cancellationToken);

        if (category is null)
        {
            return BaseResponse.NotFound(nameof(Category));
        }
        
        category.UpdateFromCreateDto(request.CategoryCreateDto);
        
        await OnCategoryUpdated(category, cancellationToken);

        try
        {
            var updated = await categoriesRepository.SaveChangesAsync(cancellationToken);

            return updated ? 
                BaseResponse.Ok() :
                BaseResponse.InternalServerError("Failed to create category");
        }
        catch (DbUpdateException exception) when 
            (exception.InnerException is SqlException { Number: 2601 })
        {
            return BaseResponse.Conflict("Category with the same name already exists.");
        }
        catch (Exception)
        {
            return BaseResponse.InternalServerError();
        }
    }

    private async Task OnCategoryUpdated(Category category, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(
            new SystemActionEvent
            {
                UserId = httpContext.UserId,
                ActionType = ActionType.Update,
                Message = $"Category \"{category.Name}\" updated"
            }, cancellationToken);
    }
}