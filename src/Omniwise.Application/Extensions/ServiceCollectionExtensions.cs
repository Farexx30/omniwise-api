using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Omniwise.Application.Services.Files;
using Omniwise.Application.Services.Notifications;


namespace Omniwise.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));

        services.AddAutoMapper(typeof(ServiceCollectionExtensions).Assembly);

        services.AddValidatorsFromAssembly(applicationAssembly)
                .AddFluentValidationAutoValidation();

        services.AddScoped<IFileService, FileService>();
        services.AddScoped<INotificationService, NotificationService>();
    }
}
