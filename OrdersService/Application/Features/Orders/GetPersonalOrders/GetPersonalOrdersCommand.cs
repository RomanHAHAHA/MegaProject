using MediatR;
using OrdersService.Domain.Dtos;

namespace OrdersService.Application.Features.Orders.GetPersonalOrders;

public record GetPersonalOrdersCommand(Guid UserId) : IRequest<List<PersonalOrderDto>>;