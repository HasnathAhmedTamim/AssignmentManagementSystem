using AssignmentManagement.Application.Features.Submissions.DTOs;

namespace AssignmentManagement.Application.Features.Submissions.Interfaces;

public interface ISubmissionService
{
    Task<List<SubmissionResponse>> GetByAssignmentIdAsync(Guid assignmentId);

    Task<List<SubmissionResponse>> GetMySubmissionsAsync();

    Task<SubmissionResponse?> GetByIdAsync(Guid id);

    Task<SubmissionResponse> SubmitAsync(CreateSubmissionRequest request);

    Task UpdateAsync(Guid id, UpdateSubmissionRequest request);

    Task GradeAsync(Guid id, GradeSubmissionRequest request);
}
