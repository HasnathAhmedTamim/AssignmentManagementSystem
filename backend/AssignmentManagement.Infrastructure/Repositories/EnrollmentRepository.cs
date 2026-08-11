using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _context;

    public EnrollmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentEnrollment>> GetAllAsync()
    {
        return await _context.StudentEnrollments
            .Include(x => x.Student)
            .Include(x => x.ClassRoom)
            .OrderBy(x => x.Student.FullName)
            .ToListAsync();
    }

    public async Task<List<StudentEnrollment>> GetByStudentIdAsync(Guid studentId)
    {
        return await _context.StudentEnrollments
            .Include(x => x.Student)
            .Include(x => x.ClassRoom)
            .Where(x => x.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<StudentEnrollment?> GetByIdAsync(Guid id)
    {
        return await _context.StudentEnrollments
            .Include(x => x.Student)
            .Include(x => x.ClassRoom)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsAsync(Guid studentId, Guid classRoomId)
    {
        return await _context.StudentEnrollments.AnyAsync(x =>
            x.StudentId == studentId && x.ClassRoomId == classRoomId);
    }

    public async Task<bool> IsStudentEnrolledInClassAsync(Guid studentId, Guid classRoomId)
    {
        return await ExistsAsync(studentId, classRoomId);
    }

    public async Task AddAsync(StudentEnrollment enrollment)
    {
        await _context.StudentEnrollments.AddAsync(enrollment);
    }

    public void Delete(StudentEnrollment enrollment)
    {
        _context.StudentEnrollments.Remove(enrollment);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
