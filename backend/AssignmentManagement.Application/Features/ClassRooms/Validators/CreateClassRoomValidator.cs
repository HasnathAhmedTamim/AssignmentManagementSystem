using AssignmentManagement.Application.Features.ClassRooms.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.ClassRooms.Validators;

public class CreateClassRoomValidator : AbstractValidator<CreateClassRoomRequest>
{
    public CreateClassRoomValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Section)
            .NotEmpty()
            .MaximumLength(50);
    }
}
