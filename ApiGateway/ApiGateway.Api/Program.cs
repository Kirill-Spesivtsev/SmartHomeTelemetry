using ApiGateway.Api.Configuration;
using ApiGateway.Api.GraphQL;
using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Services;
using ApiGateway.Infrastructure;
using ApiGateway.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CorsOptions>(builder.Configuration.GetSection(CorsOptions.SectionName));

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
builder.Services.AddCors(o =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string>() ?? "";
    o.AddDefaultPolicy(p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins(allowedOrigins.Split(",")));
});

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

app.Use(async (context, next) =>
{
    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));

    var originalToken = context.RequestAborted;

    context.RequestAborted = CancellationTokenSource
        .CreateLinkedTokenSource(originalToken, cts.Token)
        .Token;

    await next();
});

app.UseCors();


app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.Run();
