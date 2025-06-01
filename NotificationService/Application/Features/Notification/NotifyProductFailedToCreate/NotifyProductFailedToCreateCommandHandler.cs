using Common.Domain.Enums;
using Common.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using NotificationService.API.Hubs;
using NotificationService.Application.Features.Notification.NotificateProductCreated;

namespace NotificationService.Application.Features.Notification.NotifyProductFailedToCreate;

public class NotifyProductFailedToCreateCommandHandler(
    ICacheService<ProductStatusTracker> cacheService,
    IHubContext<NotificationHub> hubContext) : IRequestHandler<NotifyProductFailedToCreateCommand>
{
    public async Task Handle(NotifyProductFailedToCreateCommand request, CancellationToken cancellationToken)
    {
        await hubContext.Clients
            .User(request.UserId.ToString())
            .SendAsync("ProductCreated", new
            {
                Status = ActionStatus.Failed,
                request.ProductId,
                request.CorrelationId,
                FailedService = request.ServiceName
            }, cancellationToken);

        await cacheService.RemoveAsync(request.CorrelationId.ToString(), cancellationToken);
    }

}