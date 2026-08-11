using AssignmentManagement.Application.Common.Exceptions;
using AssignmentManagement.Application.Features.Enrollments.DTOs;
using AssignmentManagement.Application.Features.Enrollments.Interfaces;
using AssignmentManagement.Application.Interfaces;
using AssignmentManagement.Domain.Entities;
using AssignmentManagement.Domain.Enums;

namespace AssignmentManagement.Application.Features.Enrollments.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IClassRoomRepository _classRoomRepository;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        IUserRepository userRepository,
        IClassRoomRepository classRoomRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _classRoomRepository = classRoomRepository;
    }

    public async Task<List<EnrollmentResponse>> GetAllAsync()
    {
        var enrollments = await _enrollmentRepository.GetAllAsync();

        return enrollments.Select(MapToResponse).ToList();
    }

    public async Task<List<EnrollmentResponse>> GetByStudentIdAsync(Guid studentId)
    {
        var enrollments = await _enrollmentRepository.GetByStudentIdAsync(studentId);

        return enrollments.Select(MapToResponse).ToList();
    }

    public async Task<EnrollmentResponse?> GetByIdAsync(Guid id)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);

        if (enrollment == null)
            return null;

        return MapToResponse(enrollment);
    }

    public async Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request)
    {
        var student = await _userRepository.GetByIdAsync(request.StudentId);

        if (student == null)
            throw new NotFoundException("Student not found.");

        if (student.Role != Role.Student)
            throw new ConflictException("Only users with the Student role can be enrolled in a class.");

        var classRoom = await _classRoomRepository.GetByIdAsync(request.ClassRoomId);

        if (classRoom == null)
            throw new NotFoundException("Classroom not found.");

        if (await _enrollmentRepository.ExistsAsync(request.StudentId, request.ClassRoomId))
            throw new ConflictException("This student is already enrolled in this class.");

        var enrollment = new StudentEnrollment
        {
            StudentId = request.StudentId,
            ClassRoomId = request.ClassRoomId,
            CreatedAt = DateTime.UtcNow
        };

        await _enrollmentRepository.AddAsync(enrollment);
        await _enrollmentRepository.SaveChangesAsync();

        var created = await _enrollmentRepository.GetByIdAsync(enrollment.Id);

        return MapToResponse(created!);
    }

    public async Task DeleteAsync(Guid id)
    {
        var enrollment = await _enrollmentRepository.GetByIdAsync(id);

        if (enrollment == null)
            throw new NotFoundException("Enrollment not found.");

        _enrollmentRepository.Delete(enrollment);
        await _enrollmentRepository.SaveChangesAsync();
    }

    private static EnrollmentResponse MapToResponse(StudentEnrollment enrollment)
    {
        return new EnrollmentResponse
        {
            Id = enrollment.Id,
            StudentId = enrollment.StudentId,
            StudentName = enrollment.Student?.FullName ?? string.Empty,
            ClassRoomId = enrollment.ClassRoomId,
            ClassRoomName = enrollment.ClassRoom?.Name ?? string.Empty,
            ClassRoomSection = enrollment.ClassRoom?.Section ?? string.Empty,
            CreatedAt = enrollment.CreatedAt
        };
    }
}
