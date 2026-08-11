using AssignmentManagement.Application.Features.Assignments.DTOs;

namespace AssignmentManagement.Application.Features.Assignments.Interfaces;

public interface IAssignmentService
{
    Task<List<AssignmentResponse>> GetAllAsync();

    Task<AssignmentResponse?> GetByIdAsync(Guid id);

    Task<AssignmentResponse> CreateAsync(CreateAssignmentRequest request);

    Task UpdateAsync(Guid id, UpdateAssignmentRequest request);

    Task PublishAsync(Guid id);

    Task DeleteAsync(Guid id);
}
