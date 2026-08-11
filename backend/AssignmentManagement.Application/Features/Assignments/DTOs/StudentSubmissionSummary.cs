using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Features.Assignments.DTOs;

public class StudentSubmissionSummary
{
    public Guid Id { get; set; }

    public SubmissionStatus Status { get; set; }

    public int? Marks { get; set; }

    public DateTime SubmittedAt { get; set; }

    public bool CanUpdate { get; set; }
}
