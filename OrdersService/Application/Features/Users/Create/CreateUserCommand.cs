﻿using MediatR;

namespace OrdersService.Application.Features.Users.Create;

public record CreateUserCommand(
    Guid CorrelationId,
    Guid UserId,
    string NickName,
    string ConnectionId) : IRequest; 