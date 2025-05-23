using Common.Domain.Models.Results;
using MediatR;

namespace UsersService.Application.Features.Users.Login;

public record LoginUserCommand(UserLoginDto UserLoginDto) : IRequest<BaseResponse<string>>;