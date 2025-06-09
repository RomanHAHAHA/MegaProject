using Common.Application.Options;
using Common.Infrastructure.Messaging.Events.User;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ReviewsService.Infrastructure.Persistence;

namespace ReviewsService.Application.Features.Users.UpdateAvatar;

public class UpdateUserAvatarCommandHandler(
    ReviewsDbContext dbContext,
    IPublishEndpoint publisher,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<UpdateUserAvatarCommand>
{
    public async Task Handle(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        Console.WriteLine(publisher.GetType().FullName);
        var user = await dbContext.UserSnapshots
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            
        if (user is null)
        {
            await OnAvatarUpdateFailure(request, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }
        
        try
        {
            user.AvatarPath = request.AvatarPath;

            await OnAvatarUpdated(request, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await OnAvatarUpdateFailure(request, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task OnAvatarUpdated(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        await publisher.Publish(
            new UserSnapshotAvatarUpdatedEvent()
            {
                CorrelationId = request.CorrelationId,
                SenderServiceName = serviceOptions.Value.Name,
                UserId = request.UserId,
            },
            cancellationToken);
    }
    
    private async Task OnAvatarUpdateFailure(UpdateUserAvatarCommand request, CancellationToken cancellationToken)
    {
        await publisher.Publish(
            new UserSnapshotAvatarUpdateFailureEvent
            {
                CorrelationId = request.CorrelationId,
                SenderServiceName = serviceOptions.Value.Name,
                UserId = request.UserId,
            },
            cancellationToken);
    }
}