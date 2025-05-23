namespace UsersService.Application.Features.Users.Register;

public record UserRegisterDto(
    string NickName,
    string Email,
    string Password,
    string PasswordConfirm);