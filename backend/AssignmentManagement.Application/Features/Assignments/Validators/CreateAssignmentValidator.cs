using AssignmentManagement.Application.Features.Assignments.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Assignments.Validators;

public class CreateAssignmentValidator : AbstractValidator<CreateAssignmentRequest>
{
    public CreateAssignmentValidator()
    {
        RuleFor(x => x.TeacherClassSubjectId)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Deadline)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Deadline must be in the future.");

        RuleFor(x => x.MaximumMarks)
            .GreaterThan(0);
    }
}
