using AssignmentManagement.Application.Features.Enrollments.DTOs;

namespace AssignmentManagement.Application.Features.Enrollments.Interfaces;

public interface IEnrollmentService
{
    Task<List<EnrollmentResponse>> GetAllAsync();

    Task<List<EnrollmentResponse>> GetByStudentIdAsync(Guid studentId);

    Task<EnrollmentResponse?> GetByIdAsync(Guid id);

    Task<EnrollmentResponse> CreateAsync(CreateEnrollmentRequest request);

    Task DeleteAsync(Guid id);
}
