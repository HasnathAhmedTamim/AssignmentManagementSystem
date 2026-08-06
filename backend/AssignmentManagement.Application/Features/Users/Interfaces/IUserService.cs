using AssignmentManagement.Application.Features.Users.DTOs;

namespace AssignmentManagement.Application.Features.Users.Interfaces;

public interface IUserService
{
    Task<UserResponse> CreateAsync(CreateUserRequest request);

    Task<IEnumerable<UserResponse>> GetAllAsync();

    Task<UserResponse?> GetByIdAsync(Guid id);

    Task UpdateAsync(Guid id, UpdateUserRequest request);

    Task DeleteAsync(Guid id);
}