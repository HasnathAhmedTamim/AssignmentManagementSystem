using AssignmentManagement.Application.Features.Submissions.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Submissions.Validators;

public class UpdateSubmissionValidator : AbstractValidator<UpdateSubmissionRequest>
{
    public UpdateSubmissionValidator()
    {
        RuleFor(x => x.Answer)
            .NotEmpty()
            .MaximumLength(8000);
    }
}
