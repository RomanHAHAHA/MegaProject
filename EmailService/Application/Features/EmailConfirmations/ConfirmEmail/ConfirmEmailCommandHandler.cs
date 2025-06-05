using Common.Application.Options;
using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using Common.Infrastructure.Messaging.Events.Email;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;

namespace EmailService.Application.Features.EmailConfirmations.ConfirmEmail;

public class ConfirmEmailCommandHandler(
    ICacheService<string> cacheService,
    IPasswordHasher passwordHasher,
    IPublishEndpoint publishEndpoint,
    IOptions<ServiceOptions> serviceOptions) : IRequestHandler<ConfirmEmailCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var hashedCode = await cacheService.GetAsync(request.Email, cancellationToken);

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
        await publishEndpoint.Publish(
            new EmailConfirmedEvent
            {
                CorrelationId = Guid.NewGuid(),
                SenderServiceName = serviceOptions.Value.Name,
                Email = email
            }, 
            cancellationToken);
    }
}