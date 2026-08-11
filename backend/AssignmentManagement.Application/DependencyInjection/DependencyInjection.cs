using AssignmentManagement.Application.Features.Assignments.Interfaces;
using AssignmentManagement.Application.Features.Assignments.Services;
using AssignmentManagement.Application.Features.Auth.Interfaces;
using AssignmentManagement.Application.Features.Auth.Services;
using AssignmentManagement.Application.Features.ClassRooms.Interfaces;
using AssignmentManagement.Application.Features.ClassRooms.Services;
using AssignmentManagement.Application.Features.Enrollments.Interfaces;
using AssignmentManagement.Application.Features.Enrollments.Services;
using AssignmentManagement.Application.Features.Subjects.Interfaces;
using AssignmentManagement.Application.Features.Subjects.Services;
using AssignmentManagement.Application.Features.Submissions.Interfaces;
using AssignmentManagement.Application.Features.Submissions.Services;
using AssignmentManagement.Application.Features.TeacherAssignments.Interfaces;
using AssignmentManagement.Application.Features.TeacherAssignments.Services;
using AssignmentManagement.Application.Features.Users.Interfaces;
using AssignmentManagement.Application.Features.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AssignmentManagement.Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IClassRoomService, ClassRoomService>();
        services.AddScoped<ISubjectService, SubjectService>();
        services.AddScoped<ITeacherAssignmentService, TeacherAssignmentService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();
        services.AddScoped<IAssignmentService, AssignmentService>();
        services.AddScoped<ISubmissionService, SubmissionService>();

        return services;
    }
}
