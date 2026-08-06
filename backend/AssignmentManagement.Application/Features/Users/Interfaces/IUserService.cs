using AssignmentManagement.Application.Features.Users.DTOs;

namespace AssignmentManagement.Application.Features.Users.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync();

    Task<UserResponse?> GetByIdAsync(Guid id);

    Task<UserResponse> CreateAsync(CreateUserRequest request);

    Task UpdateAsync(Guid id, UpdateUserRequest request);

    Task DeleteAsync(Guid id);
}