using Common.Domain.Entities;
using Common.Domain.Interfaces;
using UsersService.Domain.Entities;

namespace UsersService.Application.Features.Users.Register;

public class UserFactory(
    IPasswordHasher passwordHasher)
{
    public User CreateFromRegisterDto(UserRegisterDto dto)
    {
        return new User
        {
            NickName = dto.NickName,
            Email = dto.Email,
            PasswordHash = passwordHasher.HashPassword(dto.Password),
            EmailConfirmed = false,
            RoleId = Role.User.Id,
        };
    }
}