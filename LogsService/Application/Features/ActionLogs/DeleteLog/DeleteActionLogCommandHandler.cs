using Common.Domain.Entities;
using Common.Domain.Models.Results;
using LogsService.Domain.Entiites;
using LogsService.Domain.Interfaces;
using MediatR;

namespace LogsService.Application.Features.ActionLogs.DeleteLog;

public class DeleteActionLogCommandHandler(
    ILogsRepository logsRepository) : IRequestHandler<DeleteActionLogCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(DeleteActionLogCommand request, CancellationToken cancellationToken)
    {
        var actionLogs = await logsRepository.GetByIdAsync(request.ActionLogId, cancellationToken);

        if (actionLogs is null)
        {
            return BaseResponse.NotFound(nameof(ActionLog));
        }
        
        var deleted = await logsRepository.DeleteAsync(actionLogs, cancellationToken);
        
        return deleted ?
            BaseResponse.Ok() :
            BaseResponse.InternalServerError("Failed to delete action log");
    }
}