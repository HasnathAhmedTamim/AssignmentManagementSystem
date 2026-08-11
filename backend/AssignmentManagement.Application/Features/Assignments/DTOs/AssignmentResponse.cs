using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Features.Assignments.DTOs;

public class AssignmentResponse
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime Deadline { get; set; }

    public int MaximumMarks { get; set; }

    public AssignmentStatus Status { get; set; }

    public Guid TeacherClassSubjectId { get; set; }

    public string ClassRoomName { get; set; } = string.Empty;

    public string SubjectName { get; set; } = string.Empty;

    public string TeacherName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public StudentSubmissionSummary? MySubmission { get; set; }
}
