using AssignmentManagement.Application.Features.Enrollments.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Enrollments.Validators;

public class CreateEnrollmentValidator : AbstractValidator<CreateEnrollmentRequest>
{
    public CreateEnrollmentValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty();

        RuleFor(x => x.ClassRoomId)
            .NotEmpty();
    }
}
