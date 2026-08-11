using AssignmentManagement.Application.Features.Subjects.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Subjects.Validators;

public class CreateSubjectValidator : AbstractValidator<CreateSubjectRequest>
{
    public CreateSubjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);
    }
}
