using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Infrastructure.Repositories;

public class TeacherAssignmentRepository : ITeacherAssignmentRepository
{
    private readonly AppDbContext _context;

    public TeacherAssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeacherClassSubject>> GetAllAsync()
    {
        return await _context.TeacherClassSubjects
            .Include(x => x.Teacher)
            .Include(x => x.ClassRoom)
            .Include(x => x.Subject)
            .OrderBy(x => x.Teacher.FullName)
            .ToListAsync();
    }

    public async Task<List<TeacherClassSubject>> GetByTeacherIdAsync(Guid teacherId)
    {
        return await _context.TeacherClassSubjects
            .Include(x => x.Teacher)
            .Include(x => x.ClassRoom)
            .Include(x => x.Subject)
            .Where(x => x.TeacherId == teacherId)
            .OrderBy(x => x.ClassRoom.Name)
            .ToListAsync();
    }

    public async Task<TeacherClassSubject?> GetByIdAsync(Guid id)
    {
        return await _context.TeacherClassSubjects
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<TeacherClassSubject?> GetByIdWithDetailsAsync(Guid id)
    {
        return await _context.TeacherClassSubjects
            .Include(x => x.Teacher)
            .Include(x => x.ClassRoom)
            .Include(x => x.Subject)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid teacherId, Guid classRoomId, Guid subjectId, Guid? excludeId = null)
    {
        return await _context.TeacherClassSubjects.AnyAsync(x =>
            x.TeacherId == teacherId &&
            x.ClassRoomId == classRoomId &&
            x.SubjectId == subjectId &&
            (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task AddAsync(TeacherClassSubject entity)
    {
        await _context.TeacherClassSubjects.AddAsync(entity);
    }

    public void Delete(TeacherClassSubject entity)
    {
        _context.TeacherClassSubjects.Remove(entity);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
