using AssignmentManagement.Domain.Entities;

namespace AssignmentManagement.Application.Interfaces;

public interface ISubmissionRepository
{
    Task<List<Submission>> GetByAssignmentIdAsync(Guid assignmentId);
    Task<List<Submission>> GetByStudentIdAsync(Guid studentId);
    Task<Submission?> GetByIdWithDetailsAsync(Guid id);
    Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId);
    Task AddAsync(Submission submission);
    void Update(Submission submission);
    Task SaveChangesAsync();
}
