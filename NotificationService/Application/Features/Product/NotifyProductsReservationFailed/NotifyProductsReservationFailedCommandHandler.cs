using MediatR;
using Microsoft.AspNetCore.SignalR;
using NotificationService.API.Hubs;

namespace NotificationService.Application.Features.Product.NotifyProductsReservationFailed;

public class NotifyProductsReservationFailedCommandHandler(
    IHubContext<NotificationHub, INotificationClient> hubContext) : 
    IRequestHandler<NotifyProductsReservationFailedCommand>
{
    public async Task Handle(NotifyProductsReservationFailedCommand request, CancellationToken cancellationToken)
    {
        await hubContext.Clients
            .User(request.UserId.ToString())
            .NotifyProductsReservationFailed(request.ProductStockInfos);
    }
}