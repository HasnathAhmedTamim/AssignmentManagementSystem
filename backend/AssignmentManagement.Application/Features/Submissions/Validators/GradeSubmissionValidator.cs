using AssignmentManagement.Application.Features.Submissions.DTOs;
using AssignmentManagement.Domain.Enums;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Submissions.Validators;

public class GradeSubmissionValidator : AbstractValidator<GradeSubmissionRequest>
{
    public GradeSubmissionValidator()
    {
        RuleFor(x => x.Marks)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Feedback)
            .MaximumLength(2000)
            .When(x => x.Feedback != null);

        RuleFor(x => x.Status)
            .IsInEnum()
            .Must(s => s == SubmissionStatus.Reviewed || s == SubmissionStatus.Pending || s == SubmissionStatus.Late)
            .WithMessage("Invalid submission status.");
    }
}
