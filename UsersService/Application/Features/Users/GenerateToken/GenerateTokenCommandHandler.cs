using Common.Domain.Models.Results;
using MediatR;
using UsersService.Domain.Entities;
using UsersService.Domain.Interfaces;

namespace UsersService.Application.Features.Users.GenerateToken;

public class GenerateTokenCommandHandler(
    IUsersRepository usersRepository,
    IJwtProvider jwtProvider) : IRequestHandler<GenerateTokenCommand, BaseResponse<string>>
{
    public async Task<BaseResponse<string>> Handle(
        GenerateTokenCommand request, 
        CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return BaseResponse<string>.NotFound(nameof(User));
        }
        
        return await jwtProvider.GenerateTokenAsync(user, cancellationToken);
    }
}