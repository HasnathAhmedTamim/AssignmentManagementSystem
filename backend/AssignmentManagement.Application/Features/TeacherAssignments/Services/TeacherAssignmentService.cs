using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Features.TeacherAssignments.DTOs;
using AssignmentManagement.Application.Features.TeacherAssignments.Interfaces;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Features.TeacherAssignments.Services;

public class TeacherAssignmentService : ITeacherAssignmentService
{
    private readonly ITeacherAssignmentRepository _teacherAssignmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClassRoomRepository _classRoomRepository;
    private readonly ISubjectRepository _subjectRepository;

    public TeacherAssignmentService(
        ITeacherAssignmentRepository teacherAssignmentRepository,
        IUserRepository userRepository,
        IClassRoomRepository classRoomRepository,
        ISubjectRepository subjectRepository)
    {
        _teacherAssignmentRepository = teacherAssignmentRepository;
        _userRepository = userRepository;
        _classRoomRepository = classRoomRepository;
        _subjectRepository = subjectRepository;
    }

    public async Task<List<TeacherAssignmentResponse>> GetAllAsync()
    {
        var items = await _teacherAssignmentRepository.GetAllAsync();

        return items.Select(MapToResponse).ToList();
    }

    public async Task<List<TeacherAssignmentResponse>> GetByTeacherIdAsync(Guid teacherId)
    {
        var items = await _teacherAssignmentRepository.GetByTeacherIdAsync(teacherId);

        return items.Select(MapToResponse).ToList();
    }

    public async Task<TeacherAssignmentResponse?> GetByIdAsync(Guid id)
    {
        var item = await _teacherAssignmentRepository.GetByIdWithDetailsAsync(id);

        if (item == null)
            return null;

        return MapToResponse(item);
    }

    public async Task<TeacherAssignmentResponse> CreateAsync(CreateTeacherAssignmentRequest request)
    {
        var teacher = await _userRepository.GetByIdAsync(request.TeacherId);

        if (teacher == null)
            throw new NotFoundException("Teacher not found.");

        if (teacher.Role != Role.Teacher)
            throw new ConflictException("Only users with the Teacher role can be assigned to a class and subject.");

        var classRoom = await _classRoomRepository.GetByIdAsync(request.ClassRoomId);

        if (classRoom == null)
            throw new NotFoundException("Classroom not found.");

        var subject = await _subjectRepository.GetByIdAsync(request.SubjectId);

        if (subject == null)
            throw new NotFoundException("Subject not found.");

        if (await _teacherAssignmentRepository.ExistsAsync(
                request.TeacherId, request.ClassRoomId, request.SubjectId))
            throw new ConflictException("This teacher is already assigned to this class and subject.");

        var entity = new TeacherClassSubject
        {
            TeacherId = request.TeacherId,
            ClassRoomId = request.ClassRoomId,
            SubjectId = request.SubjectId,
            CreatedAt = DateTime.UtcNow
        };

        await _teacherAssignmentRepository.AddAsync(entity);
        await _teacherAssignmentRepository.SaveChangesAsync();

        var created = await _teacherAssignmentRepository.GetByIdWithDetailsAsync(entity.Id);

        return MapToResponse(created!);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await _teacherAssignmentRepository.GetByIdAsync(id);

        if (entity == null)
            throw new NotFoundException("Teacher assignment not found.");

        _teacherAssignmentRepository.Delete(entity);
        await _teacherAssignmentRepository.SaveChangesAsync();
    }

    private static TeacherAssignmentResponse MapToResponse(TeacherClassSubject entity)
    {
        return new TeacherAssignmentResponse
        {
            Id = entity.Id,
            TeacherId = entity.TeacherId,
            TeacherName = entity.Teacher?.FullName ?? string.Empty,
            ClassRoomId = entity.ClassRoomId,
            ClassRoomName = entity.ClassRoom?.Name ?? string.Empty,
            ClassRoomSection = entity.ClassRoom?.Section ?? string.Empty,
            SubjectId = entity.SubjectId,
            SubjectName = entity.Subject?.Name ?? string.Empty,
            SubjectCode = entity.Subject?.Code ?? string.Empty,
            CreatedAt = entity.CreatedAt
        };
    }
}
