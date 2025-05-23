using Common.Domain.Models;
using Common.Domain.Models.Results;
using MediatR;

namespace EmailService.Application.Commands;

public record ConfirmEmailCommand(string Email, string Code) : IRequest<BaseResponse>;