using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersService.Application.Features.DeliveryLocations.GetCities;
using OrdersService.Application.Features.DeliveryLocations.GetRegions;
using OrdersService.Application.Features.DeliveryLocations.GetWarehouses;

namespace OrdersService.API.Controllers;

[Route("/api/delivery-locations")]
[ApiController]
public class DeliveryLocationsController(
    IMediator mediator) : ControllerBase
{
    [HttpGet("regions")]
    [AllowAnonymous]
    public async Task<List<object>> GetRegionsAsync(CancellationToken cancellationToken)
    {
        var query = new GetRegionsQuery();
        return await mediator.Send(query, cancellationToken);
    }
    
    [HttpGet("cities/{regionRef}")]
    [AllowAnonymous]
    public async Task<List<object>> GetCitiesAsync(
        string regionRef,
        CancellationToken cancellationToken)
    {
        var query = new GetCitiesQuery(regionRef);
        return await mediator.Send(query, cancellationToken);
    }
    
    [HttpGet("warehouses/{cityRef}")]
    [AllowAnonymous]
    public async Task<List<object>> GetRegionsAsync(
        string cityRef,
        CancellationToken cancellationToken)
    {
        var query = new GetWarehousesQuery(cityRef);
        return await mediator.Send(query, cancellationToken);
    }
}