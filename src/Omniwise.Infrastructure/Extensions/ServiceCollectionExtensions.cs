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
using Omniwise.Infrastructure.Jobs;
using Omniwise.Infrastructure.Persistence;
using Omniwise.Infrastructure.Persistence.MigrationAppliers;
using Omniwise.Infrastructure.Persistence.Seeders;
using Omniwise.Infrastructure.Repositories;
using Omniwise.Infrastructure.Services;
using Omniwise.Infrastructure.Storage;
using Quartz;

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
            .AddClaimsPrincipalFactory<CustomUserClaimsPrincipalFactory>()
            .AddEntityFrameworkStores<OmniwiseDbContext>();

        services.AddScoped<IMigrationApplier, MigrationApplier>();

        services.AddScoped<ISeeder<IdentityRole>, RoleSeeder>();
        services.AddScoped<ISeeder<User>, UserSeeder>();

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        services.AddScoped<IUsersRepository, UsersRepository>();
        services.AddScoped<ICoursesRepository, CoursesRepository>();
        services.AddScoped<IUserCourseRepository, UserCourseRepository>();
        services.AddScoped<ILecturesRepository, LecturesRepository>();
        services.AddScoped<IAssignmentsRepository, AssignmentsRepository>();
        services.AddScoped<IAssignmentSubmissionsRepository, AssignmentSubmissionsRepository>();
        services.AddScoped<IAssignmentSubmissionCommentsRepository, AssignmentSubmissionCommentsRepository>();
        services.AddScoped<INotificationsRepository, NotificationsRepository>();
        services.AddScoped<IQuartzSchedulerService, QuartzSchedulerService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.SameOwner, policy =>
                policy.Requirements.Add(new SameOwnerRequirement()))
            .AddPolicy(Policies.MustBeEnrolledInCourse, policy =>
                policy.Requirements.Add(new MustBeEnrolledInCourseRequirement()));

        services.AddSingleton<IAuthorizationHandler, SameOwnerRequirementHandler>();
        services.AddSingleton<IAuthorizationHandler, SameOwnerForAssignmentSubmissionRequirementHandler>();
        services.AddSingleton<IAuthorizationHandler, SameOwnerForAssignmentSubmissionCommentRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, MustBeEnrolledInCourseRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, MustBeEnrolledInCourseForAssignmentSubmissionRequirementHandler>();
        services.AddScoped<IAuthorizationHandler, MustBeEnrolledInCourseForAssignmentSubmissionCommentRequirementHandler>();

        services.Configure<BlobStorageSettings>(configuration.GetSection("BlobStorage"));
        services.AddScoped<IBlobStorageService, BlobStorageService>();

        services.AddQuartz(q =>
        {
            q.AddJob<CheckOverdueAssignmentJob>(opts => 
                opts.WithIdentity(CheckOverdueAssignmentJob.Name)
                .StoreDurably());

            q.UsePersistentStore(p =>
            {
                p.UseSqlServer(cfg =>
                {
                    cfg.ConnectionString = connectionString!;
                });
            });

            q.SetProperty("quartz.serializer.type", "json");
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        services.AddSingleton<QuartzSchedulerService>();
    }
}
