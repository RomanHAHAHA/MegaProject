using CartsService.Domain.Entities;
using Common.Domain.Dtos;
using Common.Domain.Entities;
using MediatR;

namespace CartsService.Application.Features.CartItems.GetUserCart;

public record GetUserCartQuery(Guid UserId) : IRequest<List<CartItemDto>>;