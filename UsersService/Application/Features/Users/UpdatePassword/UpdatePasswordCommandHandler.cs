using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using FluentValidation;
using MassTransit;
using MediatR;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Features.Users.UpdatePassword;

public class UpdatePasswordCommandHandler(
    IUsersRepository usersRepository,
    IValidator<UpdatePasswordDto> validator,
    IPublishEndpoint publishEndpoint, 
    IPasswordHasher passwordHasher) : IRequestHandler<UpdatePasswordCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        UpdatePasswordCommand request, 
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(
            request.UpdatePasswordDto, 
            cancellationToken);

        if (!validationResult.IsValid)
        {
            return BaseResponse.BadRequest(validationResult);
        }
        
        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return BaseResponse.NotFound("Unable to complete the request");
        }

        if (!passwordHasher.Verify(request.UpdatePasswordDto.OldPassword, user.PasswordHash))
        {
            await OnIncorrectPasswordEntered(user.Id, cancellationToken);
            return BaseResponse.BadRequest("Incorrect old password");
        }
        
        user.PasswordHash = passwordHasher.HashPassword(request.UpdatePasswordDto.NewPassword);
        var updated = await usersRepository.UpdateAsync(user, cancellationToken);

        if (!updated)
        {
            return BaseResponse.InternalServerError("Filed to update password");
        }

        await OnPasswordUpdated(user.Id, cancellationToken);
        return BaseResponse.Ok();
    }
    
    private async Task OnIncorrectPasswordEntered(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        var incorrectPasswordAttemptEvent = new IncorrectPasswordAttemptEvent(userId);
        await publishEndpoint.Publish(incorrectPasswordAttemptEvent, cancellationToken);
    }
    
    private async Task OnPasswordUpdated(
        Guid userId, 
        CancellationToken cancellationToken)
    {
        var passwordUpdatedEvent = new PasswordUpdatedEvent(userId);
        await publishEndpoint.Publish(passwordUpdatedEvent, cancellationToken);
    }
}