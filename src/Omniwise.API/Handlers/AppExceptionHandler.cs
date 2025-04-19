using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Omniwise.API.Extensions;

namespace Omniwise.API.Handlers;

public class AppExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetailsContext = CreateProblemDetailsContext(exception, httpContext);
        return await problemDetailsService.TryWriteAsync(problemDetailsContext);
    }

    private static ProblemDetailsContext CreateProblemDetailsContext(Exception exception, HttpContext httpContext)
    {
        var statusCode = exception.GetStatusCode();

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Detail = exception.Message,
            Type = ReasonPhrases.GetReasonPhrase(statusCode)
        };

        httpContext.Response.StatusCode = statusCode;

        var problemDetailsContext = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        };

        return problemDetailsContext;
    }
}
