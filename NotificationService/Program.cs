using MassTransit;
using NotificationService.Consumers;
using NotificationService.Hubs;
using NotificationService;
using Serilog;
using SmartHomeTelemetry.Shared.Contracts;
using Microsoft.Extensions.Options;

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

builder.Services.AddSignalR();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<TelemetryBroadcastConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        var rabbitOptions = ctx.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
        cfg.Host(rabbitOptions.Host, "/", h =>
        {
            h.Username(rabbitOptions.Username);
            h.Password(rabbitOptions.Password);
        });

        cfg.ReceiveEndpoint(TelemetryTopics.TelemetryQueue, e =>
        {
            e.ConfigureConsumer<TelemetryBroadcastConsumer>(ctx);
            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(2)));
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapHub<TelemetryHub>(TelemetryHub.HubPath);

app.Run();
