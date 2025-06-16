using Common.Domain.Models.Results;
using MediatR;

namespace UsersService.Application.Features.Users.GenerateToken;

public record GenerateTokenCommand(Guid UserId) : IRequest<ApiResponse<string>>;