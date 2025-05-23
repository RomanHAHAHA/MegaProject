using MediatR;

namespace OrdersService.Application.Features.Users.Create;

public record CreateUserCommand(
    Guid UserId,
    string NickName) : IRequest; 