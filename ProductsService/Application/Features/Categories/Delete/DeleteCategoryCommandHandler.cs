using Common.Application.Options;
using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.SystemAction;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Categories.Delete;

public class DeleteCategoryCommandHandler(
    ICategoriesRepository categoriesRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpUserContext,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<DeleteCategoryCommand, BaseResponse>
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

        return deleted ? BaseResponse.Ok() : BaseResponse.InternalServerError();
    }
    
    private async Task OnCategoryDeleted(Category category, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(
            new SystemActionEvent
            {
                CorrelationId = Guid.NewGuid(),
                SenderServiceName = serviceOptions.Value.Name,
                UserId = httpUserContext.UserId,
                ActionType = ActionType.Delete,
                Message = $"Category \"{category.Name}\" has been deleted"
            }, 
            cancellationToken);
    }
}