using AssignmentManagement.Domain.Common;
using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Domain.Entities;

public class User : AuditableEntity
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public Role Role { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties

    public ICollection<TeacherClassSubject> TeachingAssignments { get; set; }
        = new List<TeacherClassSubject>();

    public ICollection<StudentEnrollment> Enrollments { get; set; }
        = new List<StudentEnrollment>();

    public ICollection<Submission> Submissions { get; set; }
        = new List<Submission>();
}