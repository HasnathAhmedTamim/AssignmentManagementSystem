using AssignmentManagement.Domain.Entities;

namespace AssignmentManagement.Application.Interfaces;

public interface ITeacherAssignmentRepository
{
    Task<List<TeacherClassSubject>> GetAllAsync();
    Task<List<TeacherClassSubject>> GetByTeacherIdAsync(Guid teacherId);
    Task<TeacherClassSubject?> GetByIdAsync(Guid id);
    Task<TeacherClassSubject?> GetByIdWithDetailsAsync(Guid id);
    Task<bool> ExistsAsync(Guid teacherId, Guid classRoomId, Guid subjectId, Guid? excludeId = null);
    Task AddAsync(TeacherClassSubject entity);
    void Delete(TeacherClassSubject entity);
    Task SaveChangesAsync();
}
