using Omniwise.API.Handlers;

namespace Omniwise.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPresentation (this IServiceCollection services)
    {
        services.AddAuthentication();

        services.AddProblemDetails();
        services.AddExceptionHandler<AppExceptionHandler>();

        services.AddEndpointsApiExplorer();
    }
}
