using DataProcessor.Api.Configuration;
using DataProcessor.Api.Extensions;
using DataProcessor.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartHomeTelemetry.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TelemetryDbContext>(o =>
{
    var cs = builder.Configuration.GetConnectionString("Postgres");
    o.UseNpgsql(cs);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseAuthorization();

app.MapControllers();

app.Run();