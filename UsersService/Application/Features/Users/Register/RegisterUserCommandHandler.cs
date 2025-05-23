using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Features.Users.Register;

public class RegisterUserCommandHandler(
    IUsersRepository usersRepository,
    IValidator<UserRegisterDto> userRegisterValidator,
    UserFactory userFactory,
    IPublishEndpoint publishEndpoint) : IRequestHandler<RegisterUserCommand, BaseResponse<Guid>>
{
    public async Task<BaseResponse<Guid>> Handle(
        RegisterUserCommand request, 
        CancellationToken cancellationToken)
    {
        var result = userRegisterValidator.Validate(request.RegisterDto);

        if (!result.IsValid)
        {
            return BaseResponse<Guid>.BadRequest(result);
        }
        
        var user = userFactory.CreateFromRegisterDto(request.RegisterDto);

        try
        {
            var created = await usersRepository.CreateAsync(user, cancellationToken);

            if (!created)
            {
                return BaseResponse<Guid>.InternalServerError("Failed to create user");
            }

            await OnUserRegistered(user, cancellationToken);

            return user.Id;
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

    private async Task OnUserRegistered(
        User user,
        CancellationToken cancellationToken)
    {
        var userRegisteredEvent = new UserRegisteredEvent(user.Id, user.NickName, user.Email);
        await publishEndpoint.Publish(userRegisteredEvent, cancellationToken);
    }
}