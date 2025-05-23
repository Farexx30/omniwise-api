using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Omniwise.Application.Common.Interfaces.Storage;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Persistence.MigrationAppliers;
using Omniwise.Infrastructure.Persistence.Seeders;

namespace Omniwise.API.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();

        //First apply any pending migrations:
        var migrationApplier = scope.ServiceProvider.GetRequiredService<IMigrationApplier>();
        await migrationApplier.ApplyAsync();

        //Then seed the database with initial data:
        var seeders = scope.ServiceProvider.GetSeederServices();
        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }
    }

    private static IEnumerable<ISeeder> GetSeederServices(this IServiceProvider serviceProvider)
    {
        IEnumerable<ISeeder> seeders = [
            serviceProvider.GetRequiredService<ISeeder<IdentityRole>>(),
            serviceProvider.GetRequiredService<ISeeder<User>>()
        ];

        return seeders;
    }

    public static async Task InitializeBlobStorageAsync(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();

        var blobStorageService = scope.ServiceProvider.GetRequiredService<IBlobStorageService>();
        await blobStorageService.CreateBlobContainerIfNotExistsAsync();
    }
}
