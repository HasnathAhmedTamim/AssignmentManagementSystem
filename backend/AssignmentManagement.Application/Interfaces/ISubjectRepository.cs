using AssignmentManagement.Domain.Entities;

namespace AssignmentManagement.Application.Interfaces;

public interface ISubjectRepository
{
    Task<List<Subject>> GetAllAsync();
    Task<Subject?> GetByIdAsync(Guid id);
    Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null);
    Task AddAsync(Subject subject);
    void Update(Subject subject);
    void Delete(Subject subject);
    Task SaveChangesAsync();
}
