using MediatR;

namespace OrdersService.Application.Features.Users.UpdateAvatar;

public record UpdateUserAvatarCommand(
    Guid UserId,
    string AvatarPath) : IRequest;