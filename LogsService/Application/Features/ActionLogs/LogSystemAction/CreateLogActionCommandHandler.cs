using LogsService.Domain.Entiites;
using LogsService.Domain.Interfaces;
using MediatR;

namespace LogsService.Application.Features.ActionLogs.LogSystemAction;

public class CreateLogActionCommandHandler(ILogsRepository logsRepository) : 
    IRequestHandler<CreateLogActionCommand>
{
    public async Task Handle(CreateLogActionCommand request, CancellationToken cancellationToken)
    {
        var actionLog = new ActionLog
        {
            UserId = request.UserId,    
            ActionType = request.ActionType,
            Description = request.Message,
        };

        await logsRepository.CreateAsync(actionLog, cancellationToken);    
        await logsRepository.SaveChangesAsync(cancellationToken);
    }
}