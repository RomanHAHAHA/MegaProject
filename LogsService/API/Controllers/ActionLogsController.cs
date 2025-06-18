using Common.API.Authentication;
using Common.API.Extensions;
using Common.Domain.Dtos;
using Common.Domain.Enums;
using Common.Domain.Models.Results;
using LogsService.Application.Features.ActionLogs.DeleteLog;
using LogsService.Application.Features.ActionLogs.GetLogsPagedList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LogsService.API.Controllers;

[Route("api/action-logs")]
[ApiController]
public class ActionLogsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [HasPermission(PermissionEnum.ManageActionLogs)]
    public async Task<PagedList<PagedActionLogDto>> GetActionLogsAsync(
        [FromQuery] ActionLogFilter filter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams,
        CancellationToken cancellationToken)
    {
        var query = new GetActionLogsQuery(filter, sortParams, pageParams);
        return await mediator.Send(query, cancellationToken);
    }

    [HttpDelete("{actionLogId:guid}")]
    [HasPermission(PermissionEnum.ManageActionLogs)]
    public async Task<IActionResult> DeleteActionLogAsync(
        Guid actionLogId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteActionLogCommand(actionLogId);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}