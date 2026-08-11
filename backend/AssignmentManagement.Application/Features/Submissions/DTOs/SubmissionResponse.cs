using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Features.Submissions.DTOs;

public class SubmissionResponse
{
    public Guid Id { get; set; }

    public Guid AssignmentId { get; set; }

    public string AssignmentTitle { get; set; } = string.Empty;

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }

    public int? Marks { get; set; }

    public string? Feedback { get; set; }

    public SubmissionStatus Status { get; set; }

    public bool CanUpdate { get; set; }
}
