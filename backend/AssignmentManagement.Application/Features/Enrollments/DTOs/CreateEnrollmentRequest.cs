namespace AssignmentManagement.Application.Features.Enrollments.DTOs;

public class CreateEnrollmentRequest
{
    public Guid StudentId { get; set; }

    public Guid ClassRoomId { get; set; }
}
