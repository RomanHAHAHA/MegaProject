using Common.Application.Options;
using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.SystemAction;
using Common.Infrastructure.Messaging.Events.User;
using MassTransit;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Features.Users.Register;

public class RegisterUserCommandHandler(
    IUsersRepository usersRepository,
    IPasswordHasher passwordHasher,
    IPublishEndpoint publishEndpoint,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<RegisterUserCommand, BaseResponse<Guid>>
{
    public async Task<BaseResponse<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.FromRegisterDto(request.RegisterDto, passwordHasher);

        try
        {
            await usersRepository.CreateAsync(user, cancellationToken);
            await OnUserRegistered(user, request.RegisterDto.ConnectionId, cancellationToken);
            
            var created = await usersRepository.SaveChangesAsync(cancellationToken);

            return created ? user.Id : BaseResponse<Guid>.InternalServerError();
        }
        catch (DbUpdateException exception) when 
            (exception.InnerException is SqlException { Number: 2627 })
        {
            return BaseResponse<Guid>.Conflict("User with the same email already exists.");
        }
        catch (Exception)
        {
            return BaseResponse<Guid>.InternalServerError();
        }
    }

    private async Task OnUserRegistered(User user, string connectionId, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid();
        var serviceName = serviceOptions.Value.Name;
        
        await publishEndpoint.Publish(
            new SystemActionEvent
            {
                CorrelationId = correlationId,
                SenderServiceName = serviceName,
                UserId = user.Id,
                ActionType = ActionType.Create,
                Message = $"User {user.Id} registered",
            }, 
            cancellationToken); 
        
        await publishEndpoint.Publish(
            new UserRegisteredEvent
            {
                CorrelationId = correlationId,
                SenderServiceName = serviceName,
                UserId = user.Id,
                NickName = user.NickName,
                Email = user.Email,
                RegisterDate = user.CreatedAt,
                ConnectionId = connectionId,
            }, 
            cancellationToken);
    }
}