using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events;
using EmailService.Application.Commands;
using MassTransit;
using MediatR;

namespace EmailService.Application.Handlers;

public class ConfirmEmailCommandHandler(
    ICacheService<string> cacheService,
    IPasswordHasher passwordHasher,
    IPublishEndpoint publishEndpoint) : IRequestHandler<ConfirmEmailCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        ConfirmEmailCommand request, 
        CancellationToken cancellationToken)
    {
        var hashedCode = await cacheService.GetAsync(
            request.Email, 
            cancellationToken);

        if (hashedCode is null)
        {
            return BaseResponse.Conflict("Confirmation time was expired");
        }
        
        if (!passwordHasher.Verify(request.Code, hashedCode))
        {
            return BaseResponse.BadRequest("Invalid code");
        }
        
        await cacheService.RemoveAsync(request.Email, cancellationToken);
        await OnEmailConfirmed(request.Email, cancellationToken);
        
        return BaseResponse.Ok();
    }

    private async Task OnEmailConfirmed(string email, CancellationToken cancellationToken)
    {
        var emailConfirmedEvent = new EmailConfirmedEvent(email);
        await publishEndpoint.Publish(emailConfirmedEvent, cancellationToken);
    }
}