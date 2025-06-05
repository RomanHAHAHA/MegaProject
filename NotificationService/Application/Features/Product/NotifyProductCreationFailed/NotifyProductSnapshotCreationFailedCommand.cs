using MediatR;

namespace NotificationService.Application.Features.Product.NotifyProductCreationFailed;

public record NotifyProductSnapshotCreationFailedCommand(
    Guid CorrelationId,
    Guid ProductId,
    Guid UserId) : IRequest;
