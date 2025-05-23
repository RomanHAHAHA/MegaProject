using MediatR;

namespace LogsService.Application.Features.ActionLogs.LogPasswordUpdated;

public record LogPasswordUpdatedCommand(Guid UserId) : IRequest;