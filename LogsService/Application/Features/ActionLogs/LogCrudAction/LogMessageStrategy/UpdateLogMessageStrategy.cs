using Common.Domain.Extensions;
using Common.Domain.Interfaces;

namespace LogsService.Application.Features.ActionLogs.LogCrudAction.LogMessageStrategy;

public class UpdateLogMessageStrategy : ILogMessageStrategy
{
    public string GenerateMessage(string entityName, Guid id, bool success)
    {
        var formattedName = entityName.FormatEntityName();

        return success ? 
            $"Updated {formattedName} with id {id} successfully. " : 
            $"Failed to update {formattedName} with id {id}";
    }
}