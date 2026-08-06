using AssignmentManagement.Domain.Common;

namespace AssignmentManagement.Domain.Entities;

public class ClassRoom : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Section { get; set; } = string.Empty;

    // Navigation Properties

    public ICollection<StudentEnrollment> StudentEnrollments { get; set; }
        = new List<StudentEnrollment>();

    public ICollection<TeacherClassSubject> TeacherClassSubjects { get; set; }
        = new List<TeacherClassSubject>();
}