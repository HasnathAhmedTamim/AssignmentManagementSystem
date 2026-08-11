using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Infrastructure.Repositories;

public class ClassRoomRepository : IClassRoomRepository
{
    private readonly AppDbContext _context;

    public ClassRoomRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ClassRoom>> GetAllAsync()
    {
        return await _context.ClassRooms
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Section)
            .ToListAsync();
    }

    public async Task<ClassRoom?> GetByIdAsync(Guid id)
    {
        return await _context.ClassRooms.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsByNameAndSectionAsync(string name, string section, Guid? excludeId = null)
    {
        return await _context.ClassRooms.AnyAsync(x =>
            x.Name == name &&
            x.Section == section &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task AddAsync(ClassRoom classRoom)
    {
        await _context.ClassRooms.AddAsync(classRoom);
    }

    public void Update(ClassRoom classRoom)
    {
        _context.ClassRooms.Update(classRoom);
    }

    public void Delete(ClassRoom classRoom)
    {
        _context.ClassRooms.Remove(classRoom);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
