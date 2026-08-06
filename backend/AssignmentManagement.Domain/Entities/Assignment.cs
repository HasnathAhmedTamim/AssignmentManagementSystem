using AssignmentManagement.Domain.Common;
using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Domain.Entities;

public class Assignment : AuditableEntity
{
    public Guid TeacherClassSubjectId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime Deadline { get; set; }

    public int MaximumMarks { get; set; }

    public AssignmentStatus Status { get; set; }

    public TeacherClassSubject TeacherClassSubject { get; set; } = null!;

    public ICollection<Submission> Submissions { get; set; }
        = new List<Submission>();
}