using MediatR;
using OrdersService.Domain.Dtos;

namespace OrdersService.Application.Features.Orders.GetConfirmedOrders;

public record GetConfirmedOrdersQuery : IRequest<List<OrderDto>>;