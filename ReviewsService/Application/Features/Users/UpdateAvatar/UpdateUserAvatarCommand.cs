using MediatR;

namespace ReviewsService.Application.Features.Users.UpdateAvatar;

public record UpdateUserAvatarCommand(Guid UserId, string AvatarPath) : IRequest;