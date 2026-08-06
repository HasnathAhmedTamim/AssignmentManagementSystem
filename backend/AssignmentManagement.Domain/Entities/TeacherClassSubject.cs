using AssignmentManagement.Domain.Common;

namespace AssignmentManagement.Domain.Entities;

public class TeacherClassSubject : AuditableEntity
{
    public Guid TeacherId { get; set; }

    public Guid ClassRoomId { get; set; }

    public Guid SubjectId { get; set; }

    // Navigation Properties

    public User Teacher { get; set; } = null!;

    public ClassRoom ClassRoom { get; set; } = null!;

    public Subject Subject { get; set; } = null!;

    public ICollection<Assignment> Assignments { get; set; }
        = new List<Assignment>();
}