namespace AssignmentManagement.Application.Features.TeacherAssignments.DTOs;

public class CreateTeacherAssignmentRequest
{
    public Guid TeacherId { get; set; }

    public Guid ClassRoomId { get; set; }

    public Guid SubjectId { get; set; }
}
