using System;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Server.WebAPI.Extensions;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;
    private readonly IConfiguration _configuration;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    {
        _provider = provider;
        _configuration = configuration.GetRequiredSection("SwaggerSettings");

    }

    public void Configure(SwaggerGenOptions options)
    {
        // add swagger document for every API version discovered
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    private OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {

        var info = new OpenApiInfo()
        {
            Title = _configuration.GetValue<string>("Title"),
            Description = _configuration.GetValue<string>("Description"),
            Contact = new OpenApiContact()
            {
                Name = _configuration.GetValue<string>("ContactName"),
                Email = _configuration.GetValue<string>("ContactEmail"),
                Url = _configuration.GetValue<Uri>("ContactUrl")
            },
            License = new OpenApiLicense()
            {
                Name = _configuration.GetValue<string>("LicenseName"),
                Url = _configuration.GetValue<Uri>("LicenseUrl")
            },
            Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}
