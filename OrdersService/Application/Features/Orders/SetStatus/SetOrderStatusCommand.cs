using Common.Domain.Models.Results;
using MediatR;
using OrdersService.Domain.Enums;

namespace OrdersService.Application.Features.Orders.SetStatus;

public record SetOrderStatusCommand(
    Guid OrderId, 
    OrderStatus OrderStatus) : IRequest<BaseResponse>;