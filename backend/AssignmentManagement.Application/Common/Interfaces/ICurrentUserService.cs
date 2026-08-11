using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }

    string Email { get; }

    Role Role { get; }

    bool IsAuthenticated { get; }
}
