using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Features.Users.DTOs;
using AssignmentManagement.Application.Features.Users.Services;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AssignmentManagement.Tests.Users;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    private UserService CreateSut() => new(_users.Object, _hasher.Object);

    [Fact]
    public async Task Create_WithDuplicateEmail_ThrowsConflict()
    {
        _users.Setup(x => x.GetByEmailAsync("admin@school.com"))
            .ReturnsAsync(new User { Email = "admin@school.com" });

        var act = () => CreateSut().CreateAsync(new CreateUserRequest
        {
            FullName = "Dup",
            Email = "admin@school.com",
            Password = "Password1!",
            Role = Role.Student
        });

        await act.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task Delete_WhenMissing_ThrowsNotFound()
    {
        _users.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        var act = () => CreateSut().DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
