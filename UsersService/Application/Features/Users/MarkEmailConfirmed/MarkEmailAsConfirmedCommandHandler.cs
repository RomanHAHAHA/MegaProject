using Common.Domain.Models.Results;
using MediatR;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Features.Users.MarkEmailConfirmed;

public class MarkEmailAsConfirmedCommandHandler(IUsersRepository usersRepository) : 
    IRequestHandler<MarkEmailAsConfirmedCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        MarkEmailAsConfirmedCommand request, 
        CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return BaseResponse.NotFound(nameof(User));
        }

        if (user.EmailConfirmed)
        {
            return BaseResponse.Ok();
        }
        
        user.EmailConfirmed = true;
        
        usersRepository.Update(user);
        var updated = await usersRepository.SaveChangesAsync(cancellationToken);

        return updated ? 
            BaseResponse.Ok() :
            BaseResponse.InternalServerError("Failed to update user");
    }
}