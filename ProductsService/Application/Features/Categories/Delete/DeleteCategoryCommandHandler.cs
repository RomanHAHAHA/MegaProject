using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Categories.Delete;

public class DeleteCategoryCommandHandler(
    ICategoriesRepository categoriesRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpUserContext) : IRequestHandler<DeleteCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoriesRepository.GetByIdAsync(
            request.CategoryId, 
            cancellationToken);

        if (category is null)
        {
            return BaseResponse.NotFound(nameof(Category));
        }

        categoriesRepository.Delete(category);
        await OnCategoryDeleted(category, cancellationToken);
        
        var deleted = await categoriesRepository.SaveChangesAsync(cancellationToken);
        
        return deleted ? 
            BaseResponse.Ok() :
            BaseResponse.InternalServerError("Failed to delete category");
    }
    
    private async Task OnCategoryDeleted(Category category, CancellationToken cancellationToken)
    {
        var systemActionPerformed = new SystemActionEvent
        {
            UserId = httpUserContext.UserId,
            ActionType = ActionType.Delete,
            Message = $"Category \"{category.Name}\" has been deleted"
        };
        
        await publishEndpoint.Publish(systemActionPerformed, cancellationToken);
    }
}