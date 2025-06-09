using OrdersService.Domain.Entities;

namespace OrdersService.Domain.Dtos;

public class OrderDto(Order order)
{
    public Guid Id { get; set; } = order.Id;

    public string Status { get; set; } = order.Status.ToString();
    
    public UserViewDto User { get; set; } = new(order.User!);
    
    public DeliveryLocationDto DeliveryLocation { get; set; } = new(order.DeliveryLocation!);
    
    public string CreatedAt { get; set; } = $"{order.CreatedAt.ToLocalTime():dd.MM.yyyy HH:mm}";

    public List<OrderItemDto> OrderItems { get; set; } = order.OrderItems
        .Select(oi => new OrderItemDto(oi)).ToList();

    public decimal TotalPrice => OrderItems.Sum(oi => oi.Product.Price * oi.Quantity);
}

public class UserViewDto(UserSnapshot user)
{
    public Guid Id { get; set; } = user.Id;

    public string NickName { get; set; } = user.NickName;

    public string AvatarName { get; set; } = user.AvatarPath;
}