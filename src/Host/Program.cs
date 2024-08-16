using EMS20.WebApi.Infrastructure;
using EMS20.WebApi.Infrastructure.Common;
using EMS20.WebApi.Infrastructure.Logging.Serilog;
using EMS20.WebApi.Application;
using EMS20.WebApi.Host.Configurations;
using EMS20.WebApi.Host.Controllers;
using Serilog;
using Serilog.Formatting.Compact;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using PushNotifications.SignalR;
using PushNotifications.Interfaces;
using PushNotifications.Firebase;
using PushNotifications.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using EMS20.WebApi.Application.System.Settings.TimeZoneSettings;
using EMS20.WebApi.Infrastructure.Settings;

[assembly: ApiConventionType(typeof(FSHApiConventions))]

StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSignalR();

    builder.Services.AddHttpClient();

    
    /*builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAnyOriginPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    });*/
    builder.AddConfigurations().RegisterSerilog();
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CORSPolicy",
            builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed((hosts) => true));
    });
    builder.Services.AddScoped<ITimeZoneService, TimeZoneService>();
    builder.Services.AddControllers();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();
    var settings = builder.Configuration.GetSection(nameof(FirebaseSettings)).Get<FirebaseSettings>();

    // Register the FirebaseSettings with the dependency injection container
    builder.Services.AddSingleton<FirebaseSettings>(settings);
    builder.Services.AddSingleton<IFirebaseSender, FirebaseSender>();
    builder.Services.AddTransient<IJsonSerializer, DefaultCorePushJsonSerializer>();
    var app = builder.Build();
    app.UseCors("CORSPolicy");
    await app.Services.InitializeDatabasesAsync();

    app.UseInfrastructure(builder.Configuration);
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHub<EMSHub>("/EMSHub");
    });
    app.Run();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
{
    StaticLogger.EnsureInitialized();
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    StaticLogger.EnsureInitialized();
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}