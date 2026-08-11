using AssignmentManagement.Application.Features.Subjects.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Subjects.Validators;

public class UpdateSubjectValidator : AbstractValidator<UpdateSubjectRequest>
{
    public UpdateSubjectValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);
    }
}
