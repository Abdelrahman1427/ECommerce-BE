using API.Extensions;
using API.Middleware;
using Application;
using Application.Helper;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.SignalR;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .Enrich.WithProperty("Application", "ECommerce")
  .WriteTo.Console()
  .WriteTo.File(
    path: "Logs/log-.txt",
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 30,
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
  .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddHealthChecks();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationService();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// JWT Authentication (with SignalR query-string support)
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

FileUploadHelper.Initialize(app.Environment, builder.Configuration);
app.MapHealthChecks("/healthz");

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

// Swagger always accessible (no RequireAuthorization)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

try
{
    Log.Information("Starting ECommerce application");

    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ECommerceContext>();
    await context.Database.MigrateAsync();
    await ECommerceContextSeeder.SeedAsync(context, services);

    Log.Information("Database migration and seeding completed successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
