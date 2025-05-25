using System.Net;
using System.Net.Mail;
using EmailService.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace EmailService.Application.Features.EmailConfirmations.SendCode;

public class SmtpEmailSender(IOptions<SmtpOptions> options) : IEmailSender
{
    private readonly SmtpOptions _options = options.Value;
    
    public async Task SendMessageAsync(string email, string subject, string message)
    {
        using var smtpClient = new SmtpClient(_options.Server, _options.Port);
        
        smtpClient.EnableSsl = true;
        smtpClient.Credentials = new NetworkCredential(
            _options.SenderEmail, 
            _options.AppPassword);

        await smtpClient.SendMailAsync(_options.SenderEmail, email, subject, message);
    }
}