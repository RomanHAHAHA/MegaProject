using Common.API.Extensions;
using EmailService.Application.Features.EmailConfirmations.ConfirmEmail;
using EmailService.Application.Features.EmailConfirmations.SendCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmailService.API.Controllers;

[Route("api/email-confirmations")]
[ApiController]
public class EmailConfirmationsController(IMediator mediator) : ControllerBase
{
    [HttpPost("code")]
    [AllowAnonymous]
    public async Task<IActionResult> SendVerificationCodeAsync(
        SendVerificationCodeCommand command,
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync(
        ConfirmEmailCommand command, 
        CancellationToken cancellationToken = default)
    {
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
}