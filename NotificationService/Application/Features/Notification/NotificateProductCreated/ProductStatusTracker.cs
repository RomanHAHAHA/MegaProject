namespace NotificationService.Application.Features.Notification.NotificateProductCreated;

public class ProductStatusTracker
{
    public Guid CorrelationId { get; set; }
    
    public Guid ProductId { get; set; }
    
    public Dictionary<string, string> ComponentStatuses { get; set; } = new();
}