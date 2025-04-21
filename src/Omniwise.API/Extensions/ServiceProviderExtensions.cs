using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence.MigrationAppliers;
using Omniwise.Infrastructure.Seeders;

namespace Omniwise.API.Extensions;

public static class ServiceProviderExtensions
{
    public async static Task InitializeDatabase(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();

        //First apply any pending migrations:
        var migrationApplier = scope.ServiceProvider.GetMigrationApplier();
        await migrationApplier.ApplyAsync();

        //Then seed the database with initial data:
        var seeders = scope.ServiceProvider.GetSeederServices();
        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }
    }

    private static IMigrationApplier GetMigrationApplier(this IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<IMigrationApplier>();
    }

    private static IEnumerable<ISeeder> GetSeederServices(this IServiceProvider serviceProvider)
    {
        IEnumerable<ISeeder> seeders = [
            serviceProvider.GetRequiredService<ISeeder<IdentityRole>>(),
            serviceProvider.GetRequiredService<ISeeder<User>>()
        ];

        return seeders;
    }
}
