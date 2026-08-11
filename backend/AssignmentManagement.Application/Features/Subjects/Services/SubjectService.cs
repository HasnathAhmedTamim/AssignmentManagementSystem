using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Features.Subjects.DTOs;
using AssignmentManagement.Application.Features.Subjects.Interfaces;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;

namespace AssignmentManagement.Application.Features.Subjects.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<List<SubjectResponse>> GetAllAsync()
    {
        var subjects = await _subjectRepository.GetAllAsync();

        return subjects.Select(MapToResponse).ToList();
    }

    public async Task<SubjectResponse?> GetByIdAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);

        if (subject == null)
            return null;

        return MapToResponse(subject);
    }

    public async Task<SubjectResponse> CreateAsync(CreateSubjectRequest request)
    {
        if (await _subjectRepository.ExistsByCodeAsync(request.Code))
            throw new ConflictException("A subject with this code already exists.");

        var subject = new Subject
        {
            Name = request.Name,
            Code = request.Code,
            CreatedAt = DateTime.UtcNow
        };

        await _subjectRepository.AddAsync(subject);
        await _subjectRepository.SaveChangesAsync();

        return MapToResponse(subject);
    }

    public async Task UpdateAsync(Guid id, UpdateSubjectRequest request)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);

        if (subject == null)
            throw new NotFoundException("Subject not found.");

        if (await _subjectRepository.ExistsByCodeAsync(request.Code, id))
            throw new ConflictException("A subject with this code already exists.");

        subject.Name = request.Name;
        subject.Code = request.Code;
        subject.UpdatedAt = DateTime.UtcNow;

        _subjectRepository.Update(subject);
        await _subjectRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);

        if (subject == null)
            throw new NotFoundException("Subject not found.");

        _subjectRepository.Delete(subject);
        await _subjectRepository.SaveChangesAsync();
    }

    private static SubjectResponse MapToResponse(Subject subject)
    {
        return new SubjectResponse
        {
            Id = subject.Id,
            Name = subject.Name,
            Code = subject.Code,
            CreatedAt = subject.CreatedAt
        };
    }
}
