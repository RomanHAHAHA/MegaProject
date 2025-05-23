namespace OrdersService.Domain.Enums;

public enum OrderStatus
{
    Created,
    Processing,
    Shipped,
    Delivered,
    Received,
    Canceled,
}