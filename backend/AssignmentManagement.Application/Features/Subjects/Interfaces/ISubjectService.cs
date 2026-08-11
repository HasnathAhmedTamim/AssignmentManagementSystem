using AssignmentManagement.Application.Features.Subjects.DTOs;

namespace AssignmentManagement.Application.Features.Subjects.Interfaces;

public interface ISubjectService
{
    Task<List<SubjectResponse>> GetAllAsync();

    Task<SubjectResponse?> GetByIdAsync(Guid id);

    Task<SubjectResponse> CreateAsync(CreateSubjectRequest request);

    Task UpdateAsync(Guid id, UpdateSubjectRequest request);

    Task DeleteAsync(Guid id);
}
