﻿namespace Common.Domain.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);

    bool Verify(string password, string hashedPassword);
}