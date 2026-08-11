using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;
using AssignmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly AppDbContext _context;

    public AssignmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Assignment>> GetAllWithDetailsAsync()
    {
        return await QueryWithDetails()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Assignment>> GetByTeacherIdAsync(Guid teacherId)
    {
        return await QueryWithDetails()
            .Where(x => x.TeacherClassSubject.TeacherId == teacherId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Assignment>> GetPublishedForStudentAsync(Guid studentId)
    {
        var classIds = await _context.StudentEnrollments
            .Where(x => x.StudentId == studentId)
            .Select(x => x.ClassRoomId)
            .ToListAsync();

        return await QueryWithDetails()
            .Where(x =>
                x.Status == AssignmentStatus.Published &&
                classIds.Contains(x.TeacherClassSubject.ClassRoomId))
            .OrderByDescending(x => x.Deadline)
            .ToListAsync();
    }

    public async Task<Assignment?> GetByIdWithDetailsAsync(Guid id)
    {
        return await QueryWithDetails()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Assignment assignment)
    {
        await _context.Assignments.AddAsync(assignment);
    }

    public void Update(Assignment assignment)
    {
        _context.Assignments.Update(assignment);
    }

    public void Delete(Assignment assignment)
    {
        _context.Assignments.Remove(assignment);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<Assignment> QueryWithDetails()
    {
        return _context.Assignments
            .Include(x => x.TeacherClassSubject)
                .ThenInclude(x => x.Teacher)
            .Include(x => x.TeacherClassSubject)
                .ThenInclude(x => x.ClassRoom)
            .Include(x => x.TeacherClassSubject)
                .ThenInclude(x => x.Subject)
            .Include(x => x.Submissions);
    }
}
