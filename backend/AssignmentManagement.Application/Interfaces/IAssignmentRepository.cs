using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Interfaces;

public interface IAssignmentRepository
{
    Task<List<Assignment>> GetAllWithDetailsAsync();
    Task<List<Assignment>> GetByTeacherIdAsync(Guid teacherId);
    Task<List<Assignment>> GetPublishedForStudentAsync(Guid studentId);
    Task<Assignment?> GetByIdWithDetailsAsync(Guid id);
    Task AddAsync(Assignment assignment);
    void Update(Assignment assignment);
    void Delete(Assignment assignment);
    Task SaveChangesAsync();
}
