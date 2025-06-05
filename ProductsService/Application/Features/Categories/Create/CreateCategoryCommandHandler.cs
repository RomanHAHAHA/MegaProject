using Common.Application.Options;
using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.SystemAction;
using MassTransit;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductsService.Domain.Entities;
using ProductsService.Domain.Interfaces;

namespace ProductsService.Application.Features.Categories.Create;

public class CreateCategoryCommandHandler(
    ICategoriesRepository categoriesRepository,
    IPublishEndpoint publishEndpoint,
    IHttpUserContext httpUserContext,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<CreateCategoryCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.FromCreateDto(request.CategoryCreateDto);

        try
        {
            await categoriesRepository.CreateAsync(category, cancellationToken);
            await OnCategoryCreated(category, cancellationToken);
            
            var created = await categoriesRepository.SaveChangesAsync(cancellationToken);

            return created ? BaseResponse.Ok() : BaseResponse.InternalServerError();
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

    private async Task OnCategoryCreated(Category category, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(new SystemActionEvent
        {
            CorrelationId = Guid.NewGuid(),
            SenderServiceName = serviceOptions.Value.Name,
            UserId = httpUserContext.UserId,
            ActionType = ActionType.Create,
            Message = $"Category \"{category.Name}\" created"
        }, cancellationToken);
    }
}