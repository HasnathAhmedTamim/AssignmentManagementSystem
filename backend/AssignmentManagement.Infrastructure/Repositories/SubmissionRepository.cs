using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace AssignmentManagement.Infrastructure.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly AppDbContext _context;

    public SubmissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Submission>> GetByAssignmentIdAsync(Guid assignmentId)
    {
        return await QueryWithDetails()
            .Where(x => x.AssignmentId == assignmentId)
            .OrderByDescending(x => x.SubmittedAt)
            .ToListAsync();
    }

    public async Task<List<Submission>> GetByStudentIdAsync(Guid studentId)
    {
        return await QueryWithDetails()
            .Where(x => x.StudentId == studentId)
            .OrderByDescending(x => x.SubmittedAt)
            .ToListAsync();
    }

    public async Task<Submission?> GetByIdWithDetailsAsync(Guid id)
    {
        return await QueryWithDetails()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Submission?> GetByAssignmentAndStudentAsync(Guid assignmentId, Guid studentId)
    {
        return await QueryWithDetails()
            .FirstOrDefaultAsync(x =>
                x.AssignmentId == assignmentId &&
                x.StudentId == studentId);
    }

    public async Task AddAsync(Submission submission)
    {
        await _context.Submissions.AddAsync(submission);
    }

    public void Update(Submission submission)
    {
        _context.Submissions.Update(submission);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    private IQueryable<Submission> QueryWithDetails()
    {
        return _context.Submissions
            .Include(x => x.Student)
            .Include(x => x.Assignment)
                .ThenInclude(x => x.TeacherClassSubject)
                    .ThenInclude(x => x.ClassRoom)
            .Include(x => x.Assignment)
                .ThenInclude(x => x.TeacherClassSubject)
                    .ThenInclude(x => x.Subject)
            .Include(x => x.Assignment)
                .ThenInclude(x => x.TeacherClassSubject)
                    .ThenInclude(x => x.Teacher);
    }
}
