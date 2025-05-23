using Common.Domain.Enums;
using LogsService.Domain.Entiites;
using LogsService.Domain.Interfaces;
using MediatR;

namespace LogsService.Application.Features.ActionLogs.LogIncorrectPassword;

public class LogIncorrectPasswordAttemptCommandHandler(
    ILogsRepository logsRepository) : IRequestHandler<LogIncorrectPasswordAttemptCommand>
{
    public async Task Handle(
        LogIncorrectPasswordAttemptCommand request, 
        CancellationToken cancellationToken)
    {
        var actionLog = new ActionLog
        {
            UserId = request.UserId,
            ActionType = ActionType.IncorrectPasswordAttempt,
            Description = "Entered incorrect password",
        };
        
        await logsRepository.CreateAsync(actionLog, cancellationToken);   
    }
}