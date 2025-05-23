using OrdersService.Domain.Entities;

namespace OrdersService.Domain.Dtos;

public class DeliveryLocationDto(DeliveryLocation deliveryLocation)
{
    public string Region { get; set; } = deliveryLocation.Region;
    
    public string City { get; set; } = deliveryLocation.City;
    
    public string Warehouse { get; set; } = deliveryLocation.Warehouse;
}