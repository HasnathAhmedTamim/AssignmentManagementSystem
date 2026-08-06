using AssignmentManagement.Application.Features.Users.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Users.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Role)
            .IsInEnum();

        RuleFor(x => x.IsActive)
            .NotNull();
    }
}