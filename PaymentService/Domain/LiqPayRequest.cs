namespace PaymentService.Domain;

public class LiqPayRequest
{
    public string PublicKey { get; set; } = string.Empty;
    
    public string Action { get; set; } = "pay";
    
    public decimal Amount { get; set; } 
    
    public string Currency { get; set; } = "UAH";
    
    public string Description { get; set; } = string.Empty;
    
    public string OrderId { get; set; } = string.Empty;
    
    public string Version { get; set; } = "3";
    
    public string ResultUrl { get; set; } = string.Empty;
    
    public string ServerUrl { get; set; } = string.Empty;
}