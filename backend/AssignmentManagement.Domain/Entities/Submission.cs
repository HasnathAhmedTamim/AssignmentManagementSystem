using AssignmentManagement.Domain.Common;
using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Domain.Entities;

public class Submission : AuditableEntity
{
    public Guid AssignmentId { get; set; }

    public Guid StudentId { get; set; }

    public string Answer { get; set; } = string.Empty;

    public DateTime SubmittedAt { get; set; }

    public int? Marks { get; set; }

    public string? Feedback { get; set; }

    public SubmissionStatus Status { get; set; }

    public Assignment Assignment { get; set; } = null!;

    public User Student { get; set; } = null!;
}