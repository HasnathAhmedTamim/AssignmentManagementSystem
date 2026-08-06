using AssignmentManagement.Application.Features.Auth.DTOs;

namespace AssignmentManagement.Application.Features.Auth.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}