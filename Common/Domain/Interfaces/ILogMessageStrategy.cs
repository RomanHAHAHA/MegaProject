namespace Common.Domain.Interfaces;

public interface ILogMessageStrategy
{
    string GenerateMessage(string entityName, Guid id, bool success);
}