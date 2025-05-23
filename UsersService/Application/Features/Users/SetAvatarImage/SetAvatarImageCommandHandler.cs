using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Features.Users.SetAvatarImage;

public class SetAvatarImageCommandHandler(
    IUsersRepository usersRepository,
    IFileStorageService fileStorageService,
    IPublishEndpoint publishEndpoint,
    IOptions<UserImagesOptions> options) : IRequestHandler<SetAvatarImageCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        SetAvatarImageCommand request, 
        CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return BaseResponse.NotFound("Unable to complete the request.");
        }

        var result = await fileStorageService.SaveFileAsync(
            request.Image.File,
            options.Value.Path,
            cancellationToken);

        if (result.IsFailure)
        {
            return BaseResponse.InternalServerError(result.Error);
        }
        
        user.AvatarPath = result.Value;
        var updated = await usersRepository.UpdateAsync(user, cancellationToken);

        if (!updated)
        {
            return BaseResponse.InternalServerError("Failed to set avatar image.");
        }

        await OnAvatarSet(user, cancellationToken);
        return BaseResponse.Ok();
    }
    
    private async Task OnAvatarSet(
        User user,
        CancellationToken cancellationToken)
    {
        var avatarSetEvent = new UserAvatarUpdatedEvent(user.Id, user.AvatarPath!);
        await publishEndpoint.Publish(avatarSetEvent, cancellationToken);
    }
}