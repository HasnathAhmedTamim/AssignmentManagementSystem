using AssignmentManagement.Application.Features.TeacherAssignments.DTOs;

namespace AssignmentManagement.Application.Features.TeacherAssignments.Interfaces;

public interface ITeacherAssignmentService
{
    Task<List<TeacherAssignmentResponse>> GetAllAsync();

    Task<List<TeacherAssignmentResponse>> GetByTeacherIdAsync(Guid teacherId);

    Task<TeacherAssignmentResponse?> GetByIdAsync(Guid id);

    Task<TeacherAssignmentResponse> CreateAsync(CreateTeacherAssignmentRequest request);

    Task DeleteAsync(Guid id);
}
