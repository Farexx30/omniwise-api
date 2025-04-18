using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omniwise.Infrastructure.Persistence;

namespace Omniwise.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OmniwiseDb");
        services.AddDbContext<OmniwiseDbContext>(options =>
            options
            .UseSqlServer(connectionString)
            .EnableSensitiveDataLogging());
    }
}
