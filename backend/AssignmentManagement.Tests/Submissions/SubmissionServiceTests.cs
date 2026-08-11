using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Common.Interfaces;
using AssignmentManagement.Application.Features.Submissions.DTOs;
using AssignmentManagement.Application.Features.Submissions.Services;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;
using FluentAssertions;
using Moq;

namespace AssignmentManagement.Tests.Submissions;

public class SubmissionServiceTests
{
    private readonly Mock<ISubmissionRepository> _submissions = new();
    private readonly Mock<IAssignmentRepository> _assignments = new();
    private readonly Mock<IEnrollmentRepository> _enrollments = new();
    private readonly Mock<ICurrentUserService> _currentUser = new();

    private SubmissionService CreateSut() =>
        new(_submissions.Object, _assignments.Object, _enrollments.Object, _currentUser.Object);

    private static Assignment PublishedAssignment(Guid teacherId, Guid classRoomId, DateTime? deadline = null)
    {
        return new Assignment
        {
            Id = Guid.NewGuid(),
            Title = "Test Assignment",
            Status = AssignmentStatus.Published,
            MaximumMarks = 100,
            Deadline = deadline ?? DateTime.UtcNow.AddDays(7),
            TeacherClassSubject = new TeacherClassSubject
            {
                TeacherId = teacherId,
                ClassRoomId = classRoomId,
                ClassRoom = new ClassRoom { Name = "Grade 10", Section = "A" },
                Subject = new Subject { Name = "Math", Code = "M1" },
                Teacher = new User { FullName = "Teacher", Role = Role.Teacher }
            }
        };
    }

    [Fact]
    public async Task Submit_OnPublishedAssignment_CreatesPendingSubmission()
    {
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var assignment = PublishedAssignment(Guid.NewGuid(), classId);

        _currentUser.SetupGet(x => x.UserId).Returns(studentId);
        _currentUser.SetupGet(x => x.Role).Returns(Role.Student);
        _assignments.Setup(x => x.GetByIdWithDetailsAsync(assignment.Id)).ReturnsAsync(assignment);
        _enrollments.Setup(x => x.IsStudentEnrolledInClassAsync(studentId, classId)).ReturnsAsync(true);
        _submissions.Setup(x => x.GetByAssignmentAndStudentAsync(assignment.Id, studentId))
            .ReturnsAsync((Submission?)null);
        _submissions.Setup(x => x.AddAsync(It.IsAny<Submission>()))
            .Callback<Submission>(s => s.Id = Guid.NewGuid())
            .Returns(Task.CompletedTask);
        _submissions.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _submissions.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new Submission
            {
                Id = id,
                AssignmentId = assignment.Id,
                StudentId = studentId,
                Answer = "My answer",
                Status = SubmissionStatus.Pending,
                SubmittedAt = DateTime.UtcNow,
                Assignment = assignment,
                Student = new User { FullName = "Alex", Role = Role.Student }
            });

        var result = await CreateSut().SubmitAsync(new CreateSubmissionRequest
        {
            AssignmentId = assignment.Id,
            Answer = "My answer"
        });

        result.Status.Should().Be(SubmissionStatus.Pending);
        _submissions.Verify(x => x.AddAsync(It.Is<Submission>(s =>
            s.Status == SubmissionStatus.Pending && s.Answer == "My answer")), Times.Once);
    }

    [Fact]
    public async Task Submit_AfterDeadline_SetsLateStatus()
    {
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var assignment = PublishedAssignment(Guid.NewGuid(), classId, DateTime.UtcNow.AddDays(-1));

        _currentUser.SetupGet(x => x.UserId).Returns(studentId);
        _currentUser.SetupGet(x => x.Role).Returns(Role.Student);
        _assignments.Setup(x => x.GetByIdWithDetailsAsync(assignment.Id)).ReturnsAsync(assignment);
        _enrollments.Setup(x => x.IsStudentEnrolledInClassAsync(studentId, classId)).ReturnsAsync(true);
        _submissions.Setup(x => x.GetByAssignmentAndStudentAsync(assignment.Id, studentId))
            .ReturnsAsync((Submission?)null);
        _submissions.Setup(x => x.AddAsync(It.IsAny<Submission>()))
            .Callback<Submission>(s => s.Id = Guid.NewGuid())
            .Returns(Task.CompletedTask);
        _submissions.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);
        _submissions.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new Submission
            {
                Id = id,
                AssignmentId = assignment.Id,
                StudentId = studentId,
                Answer = "Late",
                Status = SubmissionStatus.Late,
                SubmittedAt = DateTime.UtcNow,
                Assignment = assignment,
                Student = new User { FullName = "Alex", Role = Role.Student }
            });

        await CreateSut().SubmitAsync(new CreateSubmissionRequest
        {
            AssignmentId = assignment.Id,
            Answer = "Late"
        });

        _submissions.Verify(x => x.AddAsync(It.Is<Submission>(s =>
            s.Status == SubmissionStatus.Late)), Times.Once);
    }

    [Fact]
    public async Task Submit_ToDraftAssignment_ThrowsConflict()
    {
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var assignment = PublishedAssignment(Guid.NewGuid(), classId);
        assignment.Status = AssignmentStatus.Draft;

        _currentUser.SetupGet(x => x.UserId).Returns(studentId);
        _currentUser.SetupGet(x => x.Role).Returns(Role.Student);
        _assignments.Setup(x => x.GetByIdWithDetailsAsync(assignment.Id)).ReturnsAsync(assignment);

        var act = () => CreateSut().SubmitAsync(new CreateSubmissionRequest
        {
            AssignmentId = assignment.Id,
            Answer = "Nope"
        });

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*draft*");
    }

    [Fact]
    public async Task Submit_WhenNotEnrolled_ThrowsForbidden()
    {
        var studentId = Guid.NewGuid();
        var classId = Guid.NewGuid();
        var assignment = PublishedAssignment(Guid.NewGuid(), classId);

        _currentUser.SetupGet(x => x.UserId).Returns(studentId);
        _currentUser.SetupGet(x => x.Role).Returns(Role.Student);
        _assignments.Setup(x => x.GetByIdWithDetailsAsync(assignment.Id)).ReturnsAsync(assignment);
        _enrollments.Setup(x => x.IsStudentEnrolledInClassAsync(studentId, classId)).ReturnsAsync(false);

        var act = () => CreateSut().SubmitAsync(new CreateSubmissionRequest
        {
            AssignmentId = assignment.Id,
            Answer = "Nope"
        });

        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Update_AfterDeadline_ThrowsConflict()
    {
        var studentId = Guid.NewGuid();
        var assignment = PublishedAssignment(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddHours(-2));
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            StudentId = studentId,
            Assignment = assignment,
            Answer = "Old"
        };

        _currentUser.SetupGet(x => x.UserId).Returns(studentId);
        _currentUser.SetupGet(x => x.Role).Returns(Role.Student);
        _submissions.Setup(x => x.GetByIdWithDetailsAsync(submission.Id)).ReturnsAsync(submission);

        var act = () => CreateSut().UpdateAsync(submission.Id, new UpdateSubmissionRequest
        {
            Answer = "New"
        });

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*deadline*");
    }

    [Fact]
    public async Task Grade_WithMarksAboveMaximum_ThrowsConflict()
    {
        var teacherId = Guid.NewGuid();
        var assignment = PublishedAssignment(teacherId, Guid.NewGuid());
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            Assignment = assignment,
            StudentId = Guid.NewGuid()
        };

        _currentUser.SetupGet(x => x.UserId).Returns(teacherId);
        _currentUser.SetupGet(x => x.Role).Returns(Role.Teacher);
        _submissions.Setup(x => x.GetByIdWithDetailsAsync(submission.Id)).ReturnsAsync(submission);

        var act = () => CreateSut().GradeAsync(submission.Id, new GradeSubmissionRequest
        {
            Marks = 150,
            Feedback = "Too high",
            Status = SubmissionStatus.Reviewed
        });

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("*Marks must be between*");
    }

    [Fact]
    public async Task Grade_ByNonOwningTeacher_ThrowsForbidden()
    {
        var assignment = PublishedAssignment(Guid.NewGuid(), Guid.NewGuid());
        var submission = new Submission
        {
            Id = Guid.NewGuid(),
            Assignment = assignment,
            StudentId = Guid.NewGuid()
        };

        _currentUser.SetupGet(x => x.UserId).Returns(Guid.NewGuid());
        _currentUser.SetupGet(x => x.Role).Returns(Role.Teacher);
        _submissions.Setup(x => x.GetByIdWithDetailsAsync(submission.Id)).ReturnsAsync(submission);

        var act = () => CreateSut().GradeAsync(submission.Id, new GradeSubmissionRequest
        {
            Marks = 80,
            Status = SubmissionStatus.Reviewed
        });

        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
