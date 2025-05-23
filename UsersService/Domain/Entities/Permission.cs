﻿namespace UsersService.Domain.Entities;

public sealed class Permission
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;
}