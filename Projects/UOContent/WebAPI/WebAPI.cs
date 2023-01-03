using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Server.WebAPI.Controllers.V1;
using Server.WebAPI.Extensions;
using Server.WebAPI.Middlewares;
using Server.WebAPI.Services;

namespace Server.WebAPI;

public class WebAPICore
{

    public static void Initialize()
    {
        EventSink.ServerStarted += EventSinkOnServerStarted;

    }

    private static void EventSinkOnServerStarted()
    {
        var builder = WebApplication.CreateBuilder();

        builder.AddConfigurations();

        builder.Host.UseSerilog((_, config) =>
        {
            config.WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] [WebAPI] {Message:lj}{NewLine}{Exception}")
                .ReadFrom.Configuration(builder.Configuration);
        });

        // Register all entity services
        // **************************************
        builder.Services.AddSingleton<IAccountService, AccountService>();
        // **************************************
        builder.Services.AddCore();
        builder.Services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(typeof(WebAPICore).Assembly));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseSwaggerWithVersioning();

        app.UseAuthorization();

        app.UseMiddleware<APIKeyMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
