using Common.Domain.Enums;
using Common.Domain.Models.Results;
using MediatR;

namespace OrdersService.Application.Features.Orders.SetStatus;

public record SetOrderStatusCommand(
    Guid OrderId, 
    OrderStatus OrderStatus) : IRequest<BaseResponse>;