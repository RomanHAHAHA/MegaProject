using MediatR;
using OrdersService.Domain.Entities;
using OrdersService.Domain.Interfaces;

namespace OrdersService.Application.Features.Users.Create;

public class CreateUserCommandHandler(
    IUsersRepository usersRepository,
    ILogger<CreateUserCommandHandler> logger) : IRequestHandler<CreateUserCommand>
{
    public async Task Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new UserSnapshot
        {
            Id = request.UserId,
            NickName = request.NickName,
        };

        await usersRepository.CreateAsync(user, cancellationToken);
        var created = await usersRepository.SaveChangesAsync(cancellationToken);
        
        var message = created ? 
            "User created successfully." :
            "Failed to create user.";
        
        logger.LogInformation(message);
    }
}