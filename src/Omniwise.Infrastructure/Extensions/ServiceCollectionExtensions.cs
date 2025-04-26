using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Omniwise.Application.Common.Interfaces;
using Omniwise.Domain.Constants;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Authorization.Requirements.MustBeEnrolledInCourse;
using Omniwise.Infrastructure.Authorization.Requirements.SameOwner;
using Omniwise.Infrastructure.Identity;
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

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<ICoursesRepository, CoursesRepository>();
        services.AddScoped<IAssignmentsRepository, AssignmentsRepository>();

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.SameOwner, policy =>
                policy.Requirements.Add(new SameOwnerRequirement()))
            .AddPolicy(Policies.MustBeEnrolledInCourse, policy =>
                policy.Requirements.Add(new MustBeEnrolledInCourseRequirement()));

        services.AddSingleton<IAuthorizationHandler, SameOwnerRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, MustBeEnrolledInCourseRequirementHandler>();
    }
}
