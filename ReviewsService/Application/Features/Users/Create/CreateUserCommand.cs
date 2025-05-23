using MediatR;

namespace ReviewsService.Application.Features.Users.Create;

public record CreateUserCommand(Guid UserId, string NickName) : IRequest; 