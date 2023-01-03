using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;


namespace Server.WebAPI;

internal static class Configurations
{
    internal static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
    {
        const string configurationsDirectory = "Configuration/WebAPI";
        builder.Configuration
            .AddJsonFile($"{configurationsDirectory}/appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/logger.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/logger.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/openapi.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/openapi.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/security.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"{configurationsDirectory}/security.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        return builder;


    }
}
