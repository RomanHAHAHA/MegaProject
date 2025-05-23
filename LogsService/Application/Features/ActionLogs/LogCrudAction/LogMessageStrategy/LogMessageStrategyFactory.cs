using Common.Domain.Enums;
using Common.Domain.Interfaces;

namespace LogsService.Application.Features.ActionLogs.LogCrudAction.LogMessageStrategy;

public static class LogMessageStrategyFactory
{
    private static readonly Dictionary<ActionType, ILogMessageStrategy> Strategies = new()
    {
        { ActionType.Create, new CreateLogMessageStrategy() },
        { ActionType.Read, new ReadLogMessageStrategy() },
        { ActionType.Update, new UpdateLogMessageStrategy() },
        { ActionType.Delete, new DeleteLogMessageStrategy() }
    };

    public static ILogMessageStrategy GetStrategy(ActionType actionType)
    {
        return Strategies.TryGetValue(actionType, out var strategy) ?
            strategy :
            throw new ArgumentException("Invalid action type");
    }
}