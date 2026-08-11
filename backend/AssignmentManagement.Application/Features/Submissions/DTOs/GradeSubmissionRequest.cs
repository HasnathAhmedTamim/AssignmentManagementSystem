using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Features.Submissions.DTOs;

public class GradeSubmissionRequest
{
    public int Marks { get; set; }

    public string? Feedback { get; set; }

    public SubmissionStatus Status { get; set; }
}
