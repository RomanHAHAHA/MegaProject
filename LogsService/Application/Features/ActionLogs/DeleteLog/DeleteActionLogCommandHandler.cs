using Common.Domain.Models.Results;
using LogsService.Domain.Entiites;
using LogsService.Domain.Interfaces;
using MediatR;

namespace LogsService.Application.Features.ActionLogs.DeleteLog;

public class DeleteActionLogCommandHandler(ILogsRepository logsRepository) : 
    IRequestHandler<DeleteActionLogCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        DeleteActionLogCommand request, 
        CancellationToken cancellationToken)
    {
        var actionLog = await logsRepository.GetByIdAsync(request.ActionLogId, cancellationToken);

        if (actionLog is null)
        {
            return BaseResponse.NotFound(nameof(ActionLog));
        }
        
        logsRepository.Delete(actionLog);
        var deleted = await logsRepository.SaveChangesAsync(cancellationToken);
        
        return deleted ?
            BaseResponse.Ok() :
            BaseResponse.InternalServerError("Failed to delete action log");
    }
}