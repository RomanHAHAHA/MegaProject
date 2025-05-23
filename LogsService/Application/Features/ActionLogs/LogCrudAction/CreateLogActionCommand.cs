using Common.Domain.Enums;
using MediatR;

namespace LogsService.Application.Features.ActionLogs.LogCrudAction;

public record CreateLogActionCommand(
    Guid UserId,
    ActionType ActionType, 
    string EntityName, 
    Guid EntityId, 
    bool Success) : IRequest;