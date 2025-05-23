using Common.Domain.Entities;
using LogsService.Application.Features.ActionLogs.LogCrudAction.LogMessageStrategy;
using LogsService.Domain.Entiites;
using LogsService.Domain.Interfaces;
using MediatR;

namespace LogsService.Application.Features.ActionLogs.LogCrudAction;

public class CreateLogActionCommandHandler(
    ILogsRepository logsRepository) : IRequestHandler<CreateLogActionCommand>
{
    public async Task Handle(CreateLogActionCommand request, CancellationToken cancellationToken)
    {
        var logStrategy = LogMessageStrategyFactory.GetStrategy(request.ActionType);
        var message = logStrategy.GenerateMessage(
            request.EntityName, 
            request.EntityId, 
            request.Success);
        
        var actionLog = new ActionLog
        {
            UserId = request.UserId,    
            ActionType = request.ActionType,
            Description = message,
        };

        await logsRepository.CreateAsync(actionLog, cancellationToken);    
    }
}