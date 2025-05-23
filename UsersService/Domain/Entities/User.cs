using Common.Domain.Abstractions;

namespace UsersService.Domain.Entities;

public class User : Entity<Guid>
{
    public string NickName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string PasswordHash { get; set; } = string.Empty;
    
    public bool EmailConfirmed { get; set; }

    public string? AvatarPath { get; set; } = string.Empty;
    
    public int RoleId { get; set; }
    
    public Role? Role { get; set; }

    public DateTime? LastLogIn { get; set; }
}