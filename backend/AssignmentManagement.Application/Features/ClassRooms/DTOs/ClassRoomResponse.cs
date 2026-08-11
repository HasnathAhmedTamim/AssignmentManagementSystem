namespace AssignmentManagement.Application.Features.ClassRooms.DTOs;

public class ClassRoomResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Section { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
