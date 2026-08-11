namespace AssignmentManagement.Application.Features.Enrollments.DTOs;

public class EnrollmentResponse
{
    public Guid Id { get; set; }

    public Guid StudentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public Guid ClassRoomId { get; set; }

    public string ClassRoomName { get; set; } = string.Empty;

    public string ClassRoomSection { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
