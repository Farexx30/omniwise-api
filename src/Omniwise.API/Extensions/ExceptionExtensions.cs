using Omniwise.Domain.Exceptions;

namespace Omniwise.API.Extensions;

public static class ExceptionExtensions
{
    public static int GetStatusCode(this Exception exception)
    {
        return exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ForbiddenException => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
