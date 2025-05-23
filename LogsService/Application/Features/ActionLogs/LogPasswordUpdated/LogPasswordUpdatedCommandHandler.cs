using Common.Domain.Entities;
using Common.Domain.Enums;
using LogsService.Domain.Entiites;
using LogsService.Domain.Interfaces;
using MediatR;

namespace LogsService.Application.Features.ActionLogs.LogPasswordUpdated;

public class LogPasswordUpdatedCommandHandler(ILogsRepository logsRepository) : 
    IRequestHandler<LogPasswordUpdatedCommand>
{
    public async Task Handle(
        LogPasswordUpdatedCommand request, 
        CancellationToken cancellationToken)
    {
        var actionLog = new ActionLog
        {
            UserId = request.UserId,
            ActionType = ActionType.Update,
            Description = "Updated password"
        };
        
        await logsRepository.CreateAsync(actionLog, cancellationToken);
    }
}