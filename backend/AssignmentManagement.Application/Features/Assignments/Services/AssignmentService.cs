using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Common.Interfaces;
using AssignmentManagement.Application.Features.Assignments.DTOs;
using AssignmentManagement.Application.Features.Assignments.Interfaces;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Features.Assignments.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly ITeacherAssignmentRepository _teacherAssignmentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICurrentUserService _currentUser;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        ITeacherAssignmentRepository teacherAssignmentRepository,
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUser)
    {
        _assignmentRepository = assignmentRepository;
        _teacherAssignmentRepository = teacherAssignmentRepository;
        _enrollmentRepository = enrollmentRepository;
        _currentUser = currentUser;
    }

    public async Task<List<AssignmentResponse>> GetAllAsync()
    {
        List<Assignment> assignments;

        if (_currentUser.Role == Role.Admin)
        {
            assignments = await _assignmentRepository.GetAllWithDetailsAsync();
        }
        else if (_currentUser.Role == Role.Teacher)
        {
            assignments = await _assignmentRepository.GetByTeacherIdAsync(_currentUser.UserId);
        }
        else if (_currentUser.Role == Role.Student)
        {
            assignments = await _assignmentRepository.GetPublishedForStudentAsync(_currentUser.UserId);
        }
        else
        {
            throw new ForbiddenException("You are not allowed to view assignments.");
        }

        return assignments
            .Select(a => MapToResponse(a, includeMySubmission: _currentUser.Role == Role.Student))
            .ToList();
    }

    public async Task<AssignmentResponse?> GetByIdAsync(Guid id)
    {
        var assignment = await _assignmentRepository.GetByIdWithDetailsAsync(id);

        if (assignment == null)
            return null;

        await EnsureCanViewAsync(assignment);

        return MapToResponse(assignment, includeMySubmission: _currentUser.Role == Role.Student);
    }

    public async Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request)
    {
        var teacherClassSubject = await _teacherAssignmentRepository.GetByIdWithDetailsAsync(
            request.TeacherClassSubjectId);

        if (teacherClassSubject == null)
            throw new NotFoundException("Teacher class-subject assignment not found.");

        EnsureCanManage(teacherClassSubject.TeacherId);

        var assignment = new Assignment
        {
            TeacherClassSubjectId = request.TeacherClassSubjectId,
            Title = request.Title,
            Description = request.Description,
            Deadline = request.Deadline,
            MaximumMarks = request.MaximumMarks,
            Status = AssignmentStatus.Draft,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = _currentUser.UserId
        };

        await _assignmentRepository.AddAsync(assignment);
        await _assignmentRepository.SaveChangesAsync();

        var created = await _assignmentRepository.GetByIdWithDetailsAsync(assignment.Id);

        return MapToResponse(created!);
    }

    public async Task UpdateAsync(Guid id, UpdateAssignmentRequest request)
    {
        var assignment = await _assignmentRepository.GetByIdWithDetailsAsync(id);

        if (assignment == null)
            throw new NotFoundException("Assignment not found.");

        EnsureCanManage(assignment.TeacherClassSubject.TeacherId);

        assignment.Title = request.Title;
        assignment.Description = request.Description;
        assignment.Deadline = request.Deadline;
        assignment.MaximumMarks = request.MaximumMarks;
        assignment.UpdatedAt = DateTime.UtcNow;
        assignment.UpdatedBy = _currentUser.UserId;

        _assignmentRepository.Update(assignment);
        await _assignmentRepository.SaveChangesAsync();
    }

    public async Task PublishAsync(Guid id)
    {
        var assignment = await _assignmentRepository.GetByIdWithDetailsAsync(id);

        if (assignment == null)
            throw new NotFoundException("Assignment not found.");

        EnsureCanManage(assignment.TeacherClassSubject.TeacherId);

        if (assignment.Status == AssignmentStatus.Published)
            throw new ConflictException("Assignment is already published.");

        assignment.Status = AssignmentStatus.Published;
        assignment.UpdatedAt = DateTime.UtcNow;
        assignment.UpdatedBy = _currentUser.UserId;

        _assignmentRepository.Update(assignment);
        await _assignmentRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var assignment = await _assignmentRepository.GetByIdWithDetailsAsync(id);

        if (assignment == null)
            throw new NotFoundException("Assignment not found.");

        EnsureCanManage(assignment.TeacherClassSubject.TeacherId);

        _assignmentRepository.Delete(assignment);
        await _assignmentRepository.SaveChangesAsync();
    }

    private void EnsureCanManage(Guid teacherId)
    {
        if (_currentUser.Role == Role.Admin)
            return;

        if (_currentUser.Role == Role.Teacher && _currentUser.UserId == teacherId)
            return;

        throw new ForbiddenException("You are not allowed to manage this assignment.");
    }

    private async Task EnsureCanViewAsync(Assignment assignment)
    {
        if (_currentUser.Role == Role.Admin)
            return;

        if (_currentUser.Role == Role.Teacher)
        {
            if (assignment.TeacherClassSubject.TeacherId != _currentUser.UserId)
                throw new ForbiddenException("You are not allowed to view this assignment.");

            return;
        }

        if (_currentUser.Role == Role.Student)
        {
            if (assignment.Status != AssignmentStatus.Published)
                throw new ForbiddenException("Draft assignments are not visible to students.");

            var classRoomId = assignment.TeacherClassSubject.ClassRoomId;
            var enrolled = await _enrollmentRepository.IsStudentEnrolledInClassAsync(
                _currentUser.UserId, classRoomId);

            if (!enrolled)
                throw new ForbiddenException("You are not enrolled in the class for this assignment.");

            return;
        }

        throw new ForbiddenException("You are not allowed to view this assignment.");
    }

    private AssignmentResponse MapToResponse(Assignment assignment, bool includeMySubmission = false)
    {
        StudentSubmissionSummary? mySubmission = null;

        if (includeMySubmission)
        {
            var submission = assignment.Submissions
                .FirstOrDefault(s => s.StudentId == _currentUser.UserId);

            if (submission != null)
            {
                mySubmission = new StudentSubmissionSummary
                {
                    Id = submission.Id,
                    Status = submission.Status,
                    Marks = submission.Marks,
                    SubmittedAt = submission.SubmittedAt,
                    CanUpdate = DateTime.UtcNow <= assignment.Deadline
                };
            }
        }

        return new AssignmentResponse
        {
            Id = assignment.Id,
            Title = assignment.Title,
            Description = assignment.Description,
            Deadline = assignment.Deadline,
            MaximumMarks = assignment.MaximumMarks,
            Status = assignment.Status,
            TeacherClassSubjectId = assignment.TeacherClassSubjectId,
            ClassRoomName = assignment.TeacherClassSubject?.ClassRoom?.Name ?? string.Empty,
            SubjectName = assignment.TeacherClassSubject?.Subject?.Name ?? string.Empty,
            TeacherName = assignment.TeacherClassSubject?.Teacher?.FullName ?? string.Empty,
            CreatedAt = assignment.CreatedAt,
            MySubmission = mySubmission
        };
    }
}
