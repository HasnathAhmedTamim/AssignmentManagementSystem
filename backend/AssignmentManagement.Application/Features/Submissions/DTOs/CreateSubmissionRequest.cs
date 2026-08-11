namespace AssignmentManagement.Application.Features.Submissions.DTOs;

public class CreateSubmissionRequest
{
    public Guid AssignmentId { get; set; }

    public string Answer { get; set; } = string.Empty;
}
