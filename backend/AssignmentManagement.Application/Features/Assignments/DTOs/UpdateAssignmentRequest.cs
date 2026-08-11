namespace AssignmentManagement.Application.Features.Assignments.DTOs;

public class UpdateAssignmentRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime Deadline { get; set; }

    public int MaximumMarks { get; set; }
}
