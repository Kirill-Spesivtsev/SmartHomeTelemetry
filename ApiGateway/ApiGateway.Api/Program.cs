using ApiGateway.Api.GraphQL;
using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Services;
using ApiGateway.Infrastructure;
using ApiGateway.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TelemetryReadDbContext>(o =>
{
    var cs = builder.Configuration.GetConnectionString("Postgres");
    o.UseNpgsql(cs);
});
builder.Services.AddScoped<ITelemetryReadRepository, TelemetryReadRepository>();
builder.Services.AddScoped<TelemetryReadService, TelemetryReadService>();

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddFiltering()
    .AddSorting()
    .AddProjections()
    .ModifyCostOptions(opt => opt.MaxTypeCost = 10000);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapGraphQL("/graphql");


app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
