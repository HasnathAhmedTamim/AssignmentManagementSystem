using AssignmentManagement.Application.Features.ClassRooms.DTOs;
using FluentValidation;

namespace AssignmentManagement.Application.Features.ClassRooms.Validators;

public class UpdateClassRoomValidator : AbstractValidator<UpdateClassRoomRequest>
{
    public UpdateClassRoomValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Section)
            .NotEmpty()
            .MaximumLength(50);
    }
}
