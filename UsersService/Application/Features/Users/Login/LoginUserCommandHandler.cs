using Common.Domain.Enums;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using FluentValidation;
using MassTransit;
using MediatR;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Features.Users.Login;

public class LoginUserCommandHandler(
    IUsersRepository usersRepository,
    IJwtProvider jwtProvider,
    IPasswordHasher passwordHasher,
    IValidator<UserLoginDto> validator,
    IPublishEndpoint publishEndpoint, 
    IMediator mediator) : IRequestHandler<LoginUserCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var result = validator.Validate(request.UserLoginDto);

        if (!result.IsValid)
        {
            return BaseResponse<string>.BadRequest(result);
        }
        
        var user = await usersRepository
            .GetByEmailAsync(request.UserLoginDto.Email, cancellationToken);

        if (user is null)
        {
            return BaseResponse<string>.NotFound("User with such email");
        }

        if (!user.EmailConfirmed)
        {
            return BaseResponse<string>.Conflict("You have to confirm your email");
        }

        if (!passwordHasher.Verify(request.UserLoginDto.Password, user.PasswordHash))
        {
            await OnIncorrectPasswordEntered(user.Id, cancellationToken);
            
            return BaseResponse<string>.BadRequest("Incorrect password");
        }

        await OnUserLoggedId(user.Id, cancellationToken);
        
        return await jwtProvider.GenerateTokenAsync(user, cancellationToken);
    }
    private Task OnUserLoggedId(Guid userId, CancellationToken cancellationToken)
        => mediator.Publish(new UserLoggedInEvent(userId), cancellationToken);
    
    private async Task OnIncorrectPasswordEntered(Guid userId, CancellationToken cancellationToken)
    {
        var systemActionEvent = new SystemActionEvent
        {
            UserId = userId,
            ActionType = ActionType.IncorrectPasswordAttempt,
            Message = "Incorrect password entered"
        };
        
        await publishEndpoint.Publish(systemActionEvent, cancellationToken);
        await usersRepository.SaveChangesAsync(cancellationToken);
    }
}