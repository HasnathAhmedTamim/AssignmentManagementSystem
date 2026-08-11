using AssignmentManagement.Application.Features.TeacherAssignments.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.TeacherAssignments.Validators;

public class CreateTeacherAssignmentValidator : AbstractValidator<CreateTeacherAssignmentRequest>
{
    public CreateTeacherAssignmentValidator()
    {
        RuleFor(x => x.TeacherId)
            .NotEmpty();

        RuleFor(x => x.ClassRoomId)
            .NotEmpty();

        RuleFor(x => x.SubjectId)
            .NotEmpty();
    }
}
