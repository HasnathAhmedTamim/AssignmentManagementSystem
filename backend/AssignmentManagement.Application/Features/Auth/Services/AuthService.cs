using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Features.Auth.DTOs;
using AssignmentManagement.Application.Features.Auth.Interfaces;
using AssignmentManagement.Application.Interfaces;

namespace AssignmentManagement.Application.Features.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
            throw new UnauthorizedAppException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedAppException("This account has been deactivated.");

        var isValidPassword = _passwordHasher.Verify(
            request.Password,
            user.PasswordHash);

        if (!isValidPassword)
            throw new UnauthorizedAppException("Invalid email or password.");

        var token = _jwtTokenGenerator.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
    }
}
