using Common.Application.Options;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.User;
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
    IOptions<UserImagesOptions> userImagesOptions,
    IOptions<ServiceOptions> serviceOptions,
    ICacheService<string> cacheService) : IRequestHandler<SetAvatarImageCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(SetAvatarImageCommand request, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return BaseResponse.NotFound("Unable to complete the request.");
        }

        var result = await fileStorageService.SaveFileAsync(
            request.Image.File,
            userImagesOptions.Value.Path,
            cancellationToken);

        if (result.IsFailure)
        {
            return BaseResponse.InternalServerError(result.Error);
        }
        
        await cacheService.SetAsync(
            $"user-avatar:{user.Id}",
            user.AvatarPath ?? string.Empty,
            TimeSpan.FromMinutes(5),
            cancellationToken);
        
        user.AvatarPath = result.Value;
        await OnAvatarSet(user, cancellationToken);

        var updated = await usersRepository.SaveChangesAsync(cancellationToken);

        if (!updated)
        {
            return BaseResponse.InternalServerError();
        }
        
        return BaseResponse.Ok();
    }
    
    private async Task OnAvatarSet(User user, CancellationToken cancellationToken)
    {
        await publishEndpoint.Publish(
            new UserAvatarUpdatedEvent
            {
                CorrelationId = Guid.NewGuid(),
                SenderServiceName = serviceOptions.Value.Name,
                UserId = user.Id,
                AvatarPath = user.AvatarPath!,
            }, 
            cancellationToken);
    }
}