using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Common.Interfaces;
using AssignmentManagement.Application.Features.Assignments.DTOs;
using AssignmentManagement.Application.Features.Assignments.Services;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AssignmentManagement.Tests.Assignments;

public class AssignmentServiceTests
{
    private readonly Mock<IAssignmentRepository> _assignments = new();
    private readonly Mock<ITeacherAssignmentRepository> _teacherAssignments = new();
    private readonly Mock<IEnrollmentRepository> _enrollments = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();

    private AssignmentService CreateSut() =>
        new(_assignments.Object, _teacherAssignments.Object, _enrollments.Object, _currentUser.Object);

    [Fact]
    public async Task Create_AsOwningTeacher_CreatesDraft()
    {
        var teacherId = Guid.NewGuid();
        var tcsId = Guid.NewGuid();

        _currentUser.SetupGet(x => x.UserId).Returns(teacherId);
        _currentUser.SetupGet(x => x.Role).Returns(Role.Teacher);

        _teacherAssignments.Setup(x => x.GetByIdWithDetailsAsync(tcsId))
            .ReturnsAsync(new TeacherClassSubject
            {
                Id = tcsId,
                TeacherId = teacherId,
                ClassRoom = new ClassRoom { Name = "G10", Section = "A" },
                Subject = new Subject { Name = "Math", Code = "M" },
                Teacher = new User { FullName = "John", Role = Role.Teacher }
            });

        _assignments.Setup(x => x.AddAsync(It.IsAny<Assignment>()))
            .Callback<Assignment>(a => a.Id = Guid.NewGuid())
            .Returns(Task.CompletedTask);
        _assignments.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _assignments.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new Assignment
            {
                Id = id,
                Title = "Algebra",
                Description = "Desc",
                Deadline = DateTime.UtcNow.AddDays(5),
                MaximumMarks = 100,
                Status = AssignmentStatus.Draft,
                TeacherClassSubjectId = tcsId,
                CreatedAt = DateTime.UtcNow,
                TeacherClassSubject = new TeacherClassSubject
                {
                    TeacherId = teacherId,
                    ClassRoom = new ClassRoom { Name = "G10", Section = "A" },
                    Subject = new Subject { Name = "Math", Code = "M" },
                    Teacher = new User { FullName = "John", Role = Role.Teacher }
                }
            });

        var result = await CreateSut().CreateAsync(new CreateAssignmentRequest
        {
            TeacherClassSubjectId = tcsId,
            Title = "Algebra",
            Description = "Desc",
            Deadline = DateTime.UtcNow.AddDays(5),
            MaximumMarks = 100
        });

        result.Status.Should().Be(AssignmentStatus.Draft);
        _assignments.Verify(x => x.AddAsync(It.Is<Assignment>(a =>
            a.Status == AssignmentStatus.Draft)), Times.Once);
    }

    [Fact]
    public async Task Create_ForAnotherTeachersClass_ThrowsForbidden()
    {
        var tcsId = Guid.NewGuid();

        _currentUser.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUser.SetupGet(x => x.Role).Returns(Role.Teacher);

        _teacherAssignments.Setup(x => x.GetByIdWithDetailsAsync(tcsId))
            .ReturnsAsync(new TeacherClassSubject
            {
                Id = tcsId,
                TeacherId = Guid.NewGuid()
            });

        var act = () => CreateSut().CreateAsync(new CreateAssignmentRequest
        {
            TeacherClassSubjectId = tcsId,
            Title = "X",
            Description = "Y",
            Deadline = DateTime.UtcNow.AddDays(1),
            MaximumMarks = 10
        });

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task GetAll_AsStudent_ReturnsOnlyPublishedForEnrolledClasses()
    {
        var studentId = Guid.NewGuid();

        _currentUser.SetupGet(x => x.UserId).Returns(studentId);
        _currentUser.SetupGet(x => x.Role).Returns(Role.Student);

        _assignments.Setup(x => x.GetPublishedForStudentAsync(studentId))
            .ReturnsAsync(new List<Assignment>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Published",
                    Status = AssignmentStatus.Published,
                    Deadline = DateTime.UtcNow.AddDays(3),
                    MaximumMarks = 50,
                    CreatedAt = DateTime.UtcNow,
                    TeacherClassSubject = new TeacherClassSubject
                    {
                        ClassRoom = new ClassRoom { Name = "G10", Section = "A" },
                        Subject = new Subject { Name = "Math", Code = "M" },
                        Teacher = new User { FullName = "John", Role = Role.Teacher }
                    },
                    Submissions = new List<Submission>()
                }
            });

        var result = await CreateSut().GetAllAsync();

        result.Should().HaveCount(1);
        result[0].Status.Should().Be(AssignmentStatus.Published);
        _assignments.Verify(x => x.GetPublishedForStudentAsync(studentId), Times.Once);
        _assignments.Verify(x => x.GetAllWithDetailsAsync(), Times.Never);
    }

    [Fact]
    public async Task Publish_SetsStatusToPublished()
    {
        var teacherId = Guid.NewGuid();
        var assignment = new Assignment
        {
            Id = Guid.NewGuid(),
            Status = AssignmentStatus.Draft,
            TeacherClassSubject = new TeacherClassSubject { TeacherId = teacherId }
        };

        _currentUser.SetupGet(x => x.UserId).Returns(teacherId);
        _currentUser.SetupGet(x => x.Role).Returns(Role.Teacher);
        _assignments.Setup(x => x.GetByIdWithDetailsAsync(assignment.Id)).ReturnsAsync(assignment);
        _assignments.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

        await CreateSut().PublishAsync(assignment.Id);

        assignment.Status.Should().Be(AssignmentStatus.Published);
    }
}
