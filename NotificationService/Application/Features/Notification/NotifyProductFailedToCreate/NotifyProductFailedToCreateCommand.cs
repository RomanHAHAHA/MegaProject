using MediatR;

namespace NotificationService.Application.Features.Notification.NotifyProductFailedToCreate;

public record NotifyProductFailedToCreateCommand(
    Guid CorrelationId,
    Guid ProductId,
    Guid UserId,
    string ServiceName) : IRequest;
