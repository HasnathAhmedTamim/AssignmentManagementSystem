using AssignmentManagement.Domain.Entities;

namespace AssignmentManagement.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}