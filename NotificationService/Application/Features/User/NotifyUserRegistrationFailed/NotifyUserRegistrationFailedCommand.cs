using MediatR;

namespace NotificationService.Application.Features.User.NotifyUserRegistrationFailed;

public record NotifyUserRegistrationFailedCommand(
    Guid CorrelationId,
    string SenderServiceName,
    string ConnectionId) : IRequest;