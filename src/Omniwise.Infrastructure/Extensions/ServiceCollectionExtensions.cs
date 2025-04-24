using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omniwise.Domain.Entities;
using Omniwise.Domain.Repositories;
using Omniwise.Infrastructure.Persistence;
using Omniwise.Infrastructure.Persistence.MigrationAppliers;
using Omniwise.Infrastructure.Persistence.Seeders;
using Omniwise.Infrastructure.Repositories;

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

        services.AddIdentityApiEndpoints<User>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<OmniwiseDbContext>();

        services.AddScoped<IMigrationApplier, MigrationApplier>();

        services.AddScoped<ISeeder<IdentityRole>, RoleSeeder>();
        services.AddScoped<ISeeder<User>, UserSeeder>();

        services.AddScoped<ICoursesRepository, CoursesRepository>();

    }
}
