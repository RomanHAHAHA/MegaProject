using Common.Domain.Extensions;
using Common.Domain.Interfaces;

namespace LogsService.Application.Features.ActionLogs.LogCrudAction.LogMessageStrategy;

public class CreateLogMessageStrategy : ILogMessageStrategy
{
    public string GenerateMessage(string entityName, Guid id, bool success)
    {
        var formattedName = entityName.FormatEntityName();

        return success ? 
            $"Created {formattedName} with id {id} successfully. " : 
            $"Failed to create {formattedName} with id {id}";
    }
}