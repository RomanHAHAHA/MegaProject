using Common.Domain.Constants;
using Common.Domain.Enums;
using Common.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using NotificationService.API.Hubs;

namespace NotificationService.Application.Features.Notification.NotificateProductCreated;

public class NotifyProductCreatedCommandHandler(
    ICacheService<ProductStatusTracker> cacheService,
    IHubContext<NotificationHub> hubContext) : IRequestHandler<NotifyProductCreatedCommand>
{
    private static readonly string[] RequiredServices = [
        ProductCreationRequiredServices.ProductsService,
        ProductCreationRequiredServices.CartsService,
        ProductCreationRequiredServices.OrdersService,
        ProductCreationRequiredServices.ReviewsService
    ];

    public async Task Handle(NotifyProductCreatedCommand request, CancellationToken cancellationToken)
    {
        var tracker = await cacheService.GetAsync(request.CorrelationId.ToString(), cancellationToken) ??
                      new ProductStatusTracker
                      {
                          CorrelationId = request.CorrelationId,
                          ProductId = request.ProductId,
                          ComponentStatuses = new Dictionary<string, string>()
                      };
        
        tracker.ComponentStatuses[request.ServiceName] = ActionStatus.Success.ToString();
        
        await cacheService.SetAsync(
            request.CorrelationId.ToString(), 
            tracker, 
            TimeSpan.FromHours(1),
            cancellationToken);

        var allRequiredSucceeded = RequiredServices.All(service =>
            tracker.ComponentStatuses.TryGetValue(service, out var status) && 
            status == ActionStatus.Success.ToString());

        if (allRequiredSucceeded)
        {
            await hubContext.Clients
                .User(request.UserId.ToString())
                .SendAsync("ProductCreated", new
                {
                    Status = "Success",
                    tracker.ProductId,
                    tracker.CorrelationId
                }, cancellationToken);

            await cacheService.RemoveAsync(request.CorrelationId.ToString(), cancellationToken);
        }
    }
}