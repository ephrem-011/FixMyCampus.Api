using FixMyCampus.Api.Enums;

namespace FixMyCampus.Api.DTOs.Tickets;

public class UserResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}