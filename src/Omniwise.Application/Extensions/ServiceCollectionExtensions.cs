using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Omniwise.Domain.Repositories;


namespace Omniwise.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);
    }
}
