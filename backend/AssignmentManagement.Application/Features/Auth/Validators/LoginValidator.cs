using AssignmentManagement.Application.Features.Auth.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.Auth.Validators;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}