using MediatR;
using OrdersService.Domain.Dtos;

namespace OrdersService.Application.Features.Orders.GetUsersOrders;

public record GetUserOrdersCommand(Guid UserId) : IRequest<List<OrderDto>>;