using AssignmentManagement.Application.Features.Assignments.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Assignments.Validators;

public class UpdateAssignmentValidator : AbstractValidator<UpdateAssignmentRequest>
{
    public UpdateAssignmentValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(4000);

        RuleFor(x => x.Deadline)
            .NotEmpty();

        RuleFor(x => x.MaximumMarks)
            .GreaterThan(0);
    }
}
