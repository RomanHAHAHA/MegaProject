using Common.Domain.Interfaces;
using Common.Infrastructure.Messaging.Events;
using MediatR;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Features.Users.SetLoginTime;

public class UserLoggedInEventHandler(
    IUsersRepository usersRepository) : INotificationHandler<UserLoggedInEvent>
{
    public async Task Handle(
        UserLoggedInEvent notification, 
        CancellationToken cancellationToken)
    {
        var user = await usersRepository
            .GetByIdAsync(notification.UserId, cancellationToken);
        
        user!.LastLogIn = DateTime.UtcNow;
        await usersRepository.UpdateAsync(user, cancellationToken);
    }
}