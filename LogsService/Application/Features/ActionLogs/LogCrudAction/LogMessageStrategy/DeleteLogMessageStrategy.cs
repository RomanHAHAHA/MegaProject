using Common.Domain.Extensions;
using Common.Domain.Interfaces;

namespace LogsService.Application.Features.ActionLogs.LogCrudAction.LogMessageStrategy;

public class DeleteLogMessageStrategy : ILogMessageStrategy
{
    public string GenerateMessage(string entityName, Guid id, bool success)
    {
        var formattedName = entityName.FormatEntityName();

        return success ? 
            $"Deleted {formattedName} with id {id} successfully. " : 
            $"Failed to delete {formattedName} with id {id}";
    }
}