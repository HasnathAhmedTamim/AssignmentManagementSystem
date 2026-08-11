using AssignmentManagement.Domain.Entities;

namespace AssignmentManagement.Application.Interfaces;

public interface IClassRoomRepository
{
    Task<List<ClassRoom>> GetAllAsync();
    Task<ClassRoom?> GetByIdAsync(Guid id);
    Task<bool> ExistsByNameAndSectionAsync(string name, string section, Guid? excludeId = null);
    Task AddAsync(ClassRoom classRoom);
    void Update(ClassRoom classRoom);
    void Delete(ClassRoom classRoom);
    Task SaveChangesAsync();
}
