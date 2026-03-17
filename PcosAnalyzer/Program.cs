using Domain.Interfaces;
using PcosAnalyzer.API.Extensions;
using PcosAnalyzer.API.Middlewares;
using Persistence.Data.ContextSeed;
using Persistence.Data.HallperClass;
using Services.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddBusinessServices(builder.Configuration);
builder.Services.AddScoped<IDataSeeding, DataSeeding>();

var app = builder.Build();

await app.SeedDatabaseAsync();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PCOS Analyzer API V1");
    c.RoutePrefix = string.Empty;
});

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();