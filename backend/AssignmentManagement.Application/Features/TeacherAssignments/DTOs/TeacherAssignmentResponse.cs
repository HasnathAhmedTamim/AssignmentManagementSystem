namespace AssignmentManagement.Application.Features.TeacherAssignments.DTOs;

public class TeacherAssignmentResponse
{
    public Guid Id { get; set; }

    public Guid TeacherId { get; set; }

    public string TeacherName { get; set; } = string.Empty;

    public Guid ClassRoomId { get; set; }

    public string ClassRoomName { get; set; } = string.Empty;

    public string ClassRoomSection { get; set; } = string.Empty;

    public Guid SubjectId { get; set; }

    public string SubjectName { get; set; } = string.Empty;

    public string SubjectCode { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
