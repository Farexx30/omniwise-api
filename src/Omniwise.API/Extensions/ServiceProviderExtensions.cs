using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Omniwise.Domain.Entities;
using Omniwise.Infrastructure.Seeders;

namespace Omniwise.API.Extensions;

public static class ServiceProviderExtensions
{
    public async static Task Seed(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
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
}
