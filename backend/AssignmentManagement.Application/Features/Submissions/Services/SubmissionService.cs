using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Common.Interfaces;
using AssignmentManagement.Application.Features.Submissions.DTOs;
using AssignmentManagement.Application.Features.Submissions.Interfaces;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Features.Submissions.Services;

public class SubmissionService : ISubmissionService
{
    private readonly ISubmissionRepository _submissionRepository;
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICurrentUserService _currentUser;

    public SubmissionService(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUser)
    {
        _submissionRepository = submissionRepository;
        _assignmentRepository = assignmentRepository;
        _enrollmentRepository = enrollmentRepository;
        _currentUser = currentUser;
    }

    public async Task<List<SubmissionResponse>> GetByAssignmentIdAsync(Guid assignmentId)
    {
        var assignment = await _assignmentRepository.GetByIdWithDetailsAsync(assignmentId);

        if (assignment == null)
            throw new NotFoundException("Assignment not found.");

        EnsureCanReview(assignment);

        var submissions = await _submissionRepository.GetByAssignmentIdAsync(assignmentId);

        return submissions.Select(s => MapToResponse(s, assignment.Deadline)).ToList();
    }

    public async Task<List<SubmissionResponse>> GetMySubmissionsAsync()
    {
        if (_currentUser.Role != Role.Student)
            throw new ForbiddenException("Only students can view their own submissions list this way.");

        var submissions = await _submissionRepository.GetByStudentIdAsync(_currentUser.UserId);

        return submissions
            .Select(s => MapToResponse(s, s.Assignment.Deadline))
            .ToList();
    }

    public async Task<SubmissionResponse?> GetByIdAsync(Guid id)
    {
        var submission = await _submissionRepository.GetByIdWithDetailsAsync(id);

        if (submission == null)
            return null;

        EnsureCanViewSubmission(submission);

        return MapToResponse(submission, submission.Assignment.Deadline);
    }

    public async Task<SubmissionResponse> SubmitAsync(CreateSubmissionRequest request)
    {
        if (_currentUser.Role != Role.Student)
            throw new ForbiddenException("Only students can submit assignments.");

        var assignment = await _assignmentRepository.GetByIdWithDetailsAsync(request.AssignmentId);

        if (assignment == null)
            throw new NotFoundException("Assignment not found.");

        if (assignment.Status != AssignmentStatus.Published)
            throw new ConflictException("Cannot submit to a draft assignment.");

        var enrolled = await _enrollmentRepository.IsStudentEnrolledInClassAsync(
            _currentUser.UserId,
            assignment.TeacherClassSubject.ClassRoomId);

        if (!enrolled)
            throw new ForbiddenException("You are not enrolled in the class for this assignment.");

        var existing = await _submissionRepository.GetByAssignmentAndStudentAsync(
            request.AssignmentId, _currentUser.UserId);

        if (existing != null)
            throw new ConflictException("You have already submitted this assignment. Update your existing submission instead.");

        var now = DateTime.UtcNow;
        var status = now > assignment.Deadline
            ? SubmissionStatus.Late
            : SubmissionStatus.Pending;

        var submission = new Submission
        {
            AssignmentId = request.AssignmentId,
            StudentId = _currentUser.UserId,
            Answer = request.Answer,
            SubmittedAt = now,
            Status = status,
            CreatedAt = now,
            CreatedBy = _currentUser.UserId
        };

        await _submissionRepository.AddAsync(submission);
        await _submissionRepository.SaveChangesAsync();

        var created = await _submissionRepository.GetByIdWithDetailsAsync(submission.Id);

        return MapToResponse(created!, assignment.Deadline);
    }

    public async Task UpdateAsync(Guid id, UpdateSubmissionRequest request)
    {
        if (_currentUser.Role != Role.Student)
            throw new ForbiddenException("Only students can update their submissions.");

        var submission = await _submissionRepository.GetByIdWithDetailsAsync(id);

        if (submission == null)
            throw new NotFoundException("Submission not found.");

        if (submission.StudentId != _currentUser.UserId)
            throw new ForbiddenException("You can only update your own submissions.");

        if (DateTime.UtcNow > submission.Assignment.Deadline)
            throw new ConflictException("Cannot update submission after the deadline.");

        submission.Answer = request.Answer;
        submission.SubmittedAt = DateTime.UtcNow;
        submission.UpdatedAt = DateTime.UtcNow;
        submission.UpdatedBy = _currentUser.UserId;

        _submissionRepository.Update(submission);
        await _submissionRepository.SaveChangesAsync();
    }

    public async Task GradeAsync(Guid id, GradeSubmissionRequest request)
    {
        var submission = await _submissionRepository.GetByIdWithDetailsAsync(id);

        if (submission == null)
            throw new NotFoundException("Submission not found.");

        EnsureCanReview(submission.Assignment);

        if (request.Marks < 0 || request.Marks > submission.Assignment.MaximumMarks)
            throw new ConflictException(
                $"Marks must be between 0 and {submission.Assignment.MaximumMarks}.");

        submission.Marks = request.Marks;
        submission.Feedback = request.Feedback;
        submission.Status = request.Status;
        submission.UpdatedAt = DateTime.UtcNow;
        submission.UpdatedBy = _currentUser.UserId;

        _submissionRepository.Update(submission);
        await _submissionRepository.SaveChangesAsync();
    }

    private void EnsureCanReview(Assignment assignment)
    {
        if (_currentUser.Role == Role.Admin)
            return;

        if (_currentUser.Role == Role.Teacher &&
            assignment.TeacherClassSubject.TeacherId == _currentUser.UserId)
            return;

        throw new ForbiddenException("Only the owning teacher or an admin can review submissions for this assignment.");
    }

    private void EnsureCanViewSubmission(Submission submission)
    {
        if (_currentUser.Role == Role.Admin)
            return;

        if (_currentUser.Role == Role.Teacher)
        {
            if (submission.Assignment.TeacherClassSubject.TeacherId != _currentUser.UserId)
                throw new ForbiddenException("You are not allowed to view this submission.");

            return;
        }

        if (_currentUser.Role == Role.Student)
        {
            if (submission.StudentId != _currentUser.UserId)
                throw new ForbiddenException("You can only view your own submissions.");

            return;
        }

        throw new ForbiddenException("You are not allowed to view this submission.");
    }

    private static SubmissionResponse MapToResponse(Submission submission, DateTime deadline)
    {
        return new SubmissionResponse
        {
            Id = submission.Id,
            AssignmentId = submission.AssignmentId,
            AssignmentTitle = submission.Assignment?.Title ?? string.Empty,
            StudentId = submission.StudentId,
            StudentName = submission.Student?.FullName ?? string.Empty,
            Answer = submission.Answer,
            SubmittedAt = submission.SubmittedAt,
            Marks = submission.Marks,
            Feedback = submission.Feedback,
            Status = submission.Status,
            CanUpdate = DateTime.UtcNow <= deadline
        };
    }
}
