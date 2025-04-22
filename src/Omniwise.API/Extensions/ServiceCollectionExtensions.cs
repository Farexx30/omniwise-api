using Microsoft.OpenApi.Models;
using Omniwise.API.Handlers;
using Serilog;

namespace Omniwise.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddPresentation (this IServiceCollection services, IHostBuilder builder)
    {
        services.AddAuthentication();

        services.AddControllers();

        services.AddSwaggerAuthorization();

        services.AddProblemDetails();
        services.AddExceptionHandler<AppExceptionHandler>();

        services.AddEndpointsApiExplorer();

        builder.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }

    private static void AddSwaggerAuthorization(this IServiceCollection services)
    {
        services.AddSwaggerGen(cfg =>
        {
            cfg.AddSecurityDefinition("bearerAuthentication", 
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });

            cfg.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference 
                        { 
                            Type = ReferenceType.SecurityScheme, 
                            Id = "bearerAuthentication"
                        }
                    },
                    []
                }
            });
        });
    }
}
