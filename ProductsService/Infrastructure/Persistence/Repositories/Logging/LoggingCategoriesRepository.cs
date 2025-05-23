using Common.Domain.Enums;
 using Common.Domain.Interfaces;
 using Common.Infrastructure.Messaging.Events;
 using MassTransit;
 using ProductsService.Domain.Entities;
 using ProductsService.Domain.Interfaces;

 namespace ProductsService.Infrastructure.Persistence.Repositories.Logging;

 public class LoggingCategoriesRepository(
     ICategoriesRepository categoriesRepository,
     IPublishEndpoint publishEndpoint,
     IHttpUserContext httpUserContext) : ICategoriesRepository
 {
     public async Task<bool> CreateAsync(
         Category entity,
         CancellationToken cancellationToken = default)
     {
         var created = await categoriesRepository.CreateAsync(entity, cancellationToken);
         await OnActionPerformed(ActionType.Create, entity.Id, created, cancellationToken);
         return created;
     }

     public async Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
     {
         return await categoriesRepository.GetAllAsync(cancellationToken);
     }

     public async Task<Category?> GetByIdAsync(
         Guid id,
         CancellationToken cancellationToken = default)
     {
         var category = await categoriesRepository.GetByIdAsync(id, cancellationToken);
         await OnActionPerformed(ActionType.Read, id, category is not null, cancellationToken);
         return category;
     }

     public async Task<bool> ExistsAsync(
         Guid id, 
         CancellationToken cancellationToken = default)
     {
         return await categoriesRepository.ExistsAsync(id, cancellationToken); 
     }

     public async Task<bool> DeleteAsync(
         Category entity,
         CancellationToken cancellationToken = default)
     {
         var deleted = await categoriesRepository.DeleteAsync(entity, cancellationToken);
         await OnActionPerformed(ActionType.Delete, entity.Id, deleted, cancellationToken);
         return deleted;
     }

     public async Task<bool> UpdateAsync(
         Category entity,
         CancellationToken cancellationToken = default)
     {
         var updated = await categoriesRepository.UpdateAsync(entity, cancellationToken);
         await OnActionPerformed(ActionType.Update, entity.Id, updated, cancellationToken);
         return updated;
     }
     
     private async Task OnActionPerformed(
         ActionType actionType,
         Guid entityId,
         bool success,
         CancellationToken cancellationToken)
     {
         var actionPerformedEvent = new DbActionPerformedEvent(
             httpUserContext.UserId,
             actionType,
             nameof(Category), 
             entityId,
             success);

         await publishEndpoint.Publish(actionPerformedEvent, cancellationToken);
     }
 }