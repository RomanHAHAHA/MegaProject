using Common.Domain.Interfaces;
using Common.Domain.Models.Results;
using EmailService.Application.Commands;
using EmailService.Domain.Interfaces;
using MediatR;

namespace EmailService.Application.Handlers;

public class SendVerificationCodeCommandHandler(
    IVerificationCodeGenerator verificationCodeGenerator,
    IEmailSender emailSender, 
    IPasswordHasher passwordHasher,
    ICacheService<string> cacheService) : IRequestHandler<SendVerificationCodeCommand, BaseResponse>
{
    public async Task<BaseResponse> Handle(
        SendVerificationCodeCommand request, 
        CancellationToken cancellationToken)
    {
        var code = verificationCodeGenerator.Generate();
        var hashedCode = passwordHasher.HashPassword(code);
        
        try
        {
            await cacheService.SetAsync(
                request.Email,
                hashedCode,
                TimeSpan.FromMinutes(3),
                cancellationToken);

            await emailSender.SendMessageAsync(
                request.Email, 
                "Verification Code", 
                $"Your verification code is: {code}");

            return BaseResponse.Ok();
        }
        catch (Exception)
        {
            return BaseResponse.InternalServerError();
        }    
    }
}