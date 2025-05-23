using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;

namespace EmailService.Application.Commands;

public record SendVerificationCodeCommand(string Email) : IRequest<BaseResponse>;