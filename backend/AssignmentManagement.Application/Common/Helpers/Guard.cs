using AssignmentManagement.Application.Common.Exceptions;

namespace AssignmentManagement.Application.Common.Helpers;

public static class Guard
{
    public static T NotNull<T>(T? value, string message) where T : class
    {
        if (value is null)
            throw new NotFoundException(message);

        return value;
    }
}
