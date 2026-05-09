using DataProcessor.Api.Configuration;
using DataProcessor.Api.Extensions;
using DataProcessor.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SmartHomeTelemetry.Shared.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));

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

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<TelemetryEventConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var rabbitOptions = ctx.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value;
        cfg.Host(rabbitOptions.Host, "/", h =>
        {
            h.Username(rabbitOptions.Username);
            h.Password(rabbitOptions.Password);
        });

        cfg.ReceiveEndpoint(TelemetryTopics.TelemetryQueue, e =>
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
        });
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseAuthorization();

await app.ApplyMigrationsAsync();

app.Run();