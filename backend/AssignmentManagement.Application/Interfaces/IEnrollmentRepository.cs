using AssignmentManagement.Domain.Entities;

namespace AssignmentManagement.Application.Interfaces;

public interface IEnrollmentRepository
{
    Task<List<StudentEnrollment>> GetAllAsync();
    Task<List<StudentEnrollment>> GetByStudentIdAsync(Guid studentId);
    Task<StudentEnrollment?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid studentId, Guid classRoomId);
    Task<bool> IsStudentEnrolledInClassAsync(Guid studentId, Guid classRoomId);
    Task AddAsync(StudentEnrollment enrollment);
    void Delete(StudentEnrollment enrollment);
    Task SaveChangesAsync();
}
