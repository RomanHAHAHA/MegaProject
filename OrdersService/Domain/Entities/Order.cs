using Common.Domain.Abstractions;
using Common.Domain.Dtos;
using OrdersService.Domain.Dtos;
using OrdersService.Domain.Enums;

namespace OrdersService.Domain.Entities;

public class Order : Entity<Guid>
{
    public Guid UserId { get; set; }

    public UserSnapshot? User { get; set; }

    public OrderStatus Status { get; set; }

    public DeliveryLocation? DeliveryLocation { get; set; }

    public List<OrderItem> OrderItems { get; set; } = [];

    private Order() { }

    public Order(Guid userId)
    {
        UserId = userId;
        Status = OrderStatus.Created;
    }

    public void AddDeliveryLocation(DeliveryLocationCreateDto deliveryLocation)
    {
        DeliveryLocation = new DeliveryLocation
        {
            Region = deliveryLocation.Region,
            City = deliveryLocation.City,
            Warehouse = deliveryLocation.Warehouse,
        };
    }
    
    public void AddOrderItems(List<CartItemDto> cartItems)
    {
        OrderItems.AddRange(cartItems.Select(cartItem => new OrderItem(UserId, cartItem)));
    }
}