using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Features.Users.DTOs;

public class CreateUserRequest
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public Role Role { get; set; }
}