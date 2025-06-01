using Common.Domain.Enums;
using MediatR;

namespace NotificationService.Application.Features.Notification.NotificateProductCreated;

public record NotifyProductCreatedCommand(
    Guid CorrelationId,
    Guid ProductId,
    Guid UserId,
    string ServiceName) : IRequest;