using AssignmentManagement.Domain.Common;

namespace AssignmentManagement.Domain.Entities;

public class StudentEnrollment : AuditableEntity
{
    public Guid StudentId { get; set; }

    public Guid ClassRoomId { get; set; }

    public User Student { get; set; } = null!;

    public ClassRoom ClassRoom { get; set; } = null!;
}