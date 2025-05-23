using MediatR;

namespace LogsService.Application.Features.ActionLogs.LogIncorrectPassword;

public record LogIncorrectPasswordAttemptCommand(Guid UserId) : IRequest;