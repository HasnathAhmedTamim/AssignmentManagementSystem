using AssignmentManagement.Api.Extensions;
using AssignmentManagement.Api.Middleware;
using AssignmentManagement.Application.DependencyInjection;
using AssignmentManagement.Infrastructure.DependencyInjection;
using AssignmentManagement.Infrastructure.Persistence;
using AssignmentManagement.Infrastructure.Seed;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .Enrich.FromLogContext());

builder.Services
    .AddApiServices()
    .AddSwaggerDocumentation()
    .AddFrontendCors()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "Assignment Management API";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Assignment Management API v1");
    });
}

app.UseCors("Frontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(context);
}

app.Run();

public partial class Program;
