using AssignmentManagement.Application.Features.Submissions.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Submissions.Validators;

public class CreateSubmissionValidator : AbstractValidator<CreateSubmissionRequest>
{
    public CreateSubmissionValidator()
    {
        RuleFor(x => x.AssignmentId)
            .NotEmpty();

        RuleFor(x => x.Answer)
            .NotEmpty()
            .MaximumLength(8000);
    }
}
