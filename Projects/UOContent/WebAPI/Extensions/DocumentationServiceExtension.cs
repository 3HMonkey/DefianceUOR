using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Server.WebAPI.Extensions;

public static class DocumentationServiceExtension
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services.AddVersioning()
            .AddSwaggerVersioning();
    }

    private static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(
            setup =>
            {
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.AssumeDefaultVersionWhenUnspecified = true;
                setup.ReportApiVersions = true;
            }
        );

        services.AddVersionedApiExplorer(
            setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.SubstituteApiVersionInUrl = true;
            }
        );

        return services;
    }

    private static IServiceCollection AddSwaggerVersioning(this IServiceCollection services)
    {
        services.AddSwaggerGen(
            options =>
            {
                options.AddSecurityDefinition(
                    "ApiKey",
                    new OpenApiSecurityScheme
                    {
                        Description = "ApiKey must appear in header",
                        Type = SecuritySchemeType.ApiKey,
                        Name = "XApiKey",
                        In = ParameterLocation.Header,
                        Scheme = "ApiKeyScheme"
                    }
                );
                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement
                {
                    { key, new List<string>() }
                };
                options.AddSecurityRequirement(requirement);
            }
        );
        services.ConfigureOptions<ConfigureSwaggerOptions>();

        return services;
    }
}
