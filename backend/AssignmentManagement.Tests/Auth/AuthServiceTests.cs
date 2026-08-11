using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Common.Interfaces;
using AssignmentManagement.Application.Features.Auth.DTOs;
using AssignmentManagement.Application.Features.Auth.Services;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AssignmentManagement.Tests.Auth;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IJwtTokenGenerator> _jwt = new();

    private AuthService CreateSut() =>
        new(_users.Object, _hasher.Object, _jwt.Object);

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenAndUser()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "teacher@school.com",
            FullName = "John Teacher",
            PasswordHash = "hash",
            Role = Role.Teacher,
            IsActive = true
        };

        _users.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _hasher.Setup(x => x.Verify("Teacher@123", "hash")).Returns(true);
        _jwt.Setup(x => x.GenerateToken(user)).Returns("jwt-token");

        var result = await CreateSut().LoginAsync(new LoginRequest
        {
            Email = user.Email,
            Password = "Teacher@123"
        });

        result.Token.Should().Be("jwt-token");
        result.Role.Should().Be(Role.Teacher);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ThrowsUnauthorized()
    {
        var user = new User
        {
            Email = "teacher@school.com",
            PasswordHash = "hash",
            Role = Role.Teacher
        };

        _users.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _hasher.Setup(x => x.Verify(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        var act = () => CreateSut().LoginAsync(new LoginRequest
        {
            Email = user.Email,
            Password = "wrong"
        });

        await act.Should().ThrowAsync<UnauthorizedAppException>();
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ThrowsUnauthorized()
    {
        _users.Setup(x => x.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        var act = () => CreateSut().LoginAsync(new LoginRequest
        {
            Email = "missing@school.com",
            Password = "x"
        });

        await act.Should().ThrowAsync<UnauthorizedAppException>();
    }
}
