using AssignmentManagement.Domain.Common;

namespace AssignmentManagement.Domain.Entities;

public class Subject : AuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public ICollection<TeacherClassSubject> TeacherClassSubjects { get; set; }
        = new List<TeacherClassSubject>();
}