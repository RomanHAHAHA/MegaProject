using Common.Domain.Extensions;
using Common.Domain.Interfaces;

namespace LogsService.Application.Features.ActionLogs.LogCrudAction.LogMessageStrategy;

public class ReadLogMessageStrategy : ILogMessageStrategy
{
    public string GenerateMessage(string entityName, Guid id, bool success)
    {
        var formattedName = entityName.FormatEntityName();

        return success ? 
            $"Successfully retrieved {formattedName} with ID {id}." : 
            $"Failed to retrieve {formattedName} with ID {id}.";
    }
}