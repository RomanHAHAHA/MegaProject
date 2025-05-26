using Common.Domain.Dtos;
using Common.Domain.Models.Results;
using MediatR;

namespace OrdersService.Application.Features.Products.UpdateStockQuantity;

public record UpdateStockQuantityCommand(List<CartItemDto> CartItems) : IRequest<BaseResponse>;