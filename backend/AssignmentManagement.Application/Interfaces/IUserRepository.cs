using AssignmentManagement.Domain.Entities;

namespace AssignmentManagement.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByIdAsync(Guid id);

    Task AddAsync(User user);

    Task SaveChangesAsync();
}