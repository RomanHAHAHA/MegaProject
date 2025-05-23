using Common.Domain.Abstractions;

namespace Common.Domain.Entities;

public class OutboxMessage : Entity<Guid>
{
    public string Type { get; set; } = string.Empty;
    
    public string Payload { get; set; } = string.Empty;
    
    public bool IsProcessed { get; set; }
    
    public DateTime? ProcessedOn { get; set; }
}