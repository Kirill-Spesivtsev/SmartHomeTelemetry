using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;
using SmartHomeTelemetry.Shared.Contracts;

namespace NotificationService.Consumers;

public class TelemetryBroadcastConsumer : IConsumer<TelemetryBatchEvent>
{
    private readonly IHubContext<TelemetryHub> _hub;
    private readonly ILogger<TelemetryBroadcastConsumer> _logger;

    public TelemetryBroadcastConsumer(IHubContext<TelemetryHub> hub, ILogger<TelemetryBroadcastConsumer> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TelemetryBatchEvent> context)
    {
        await _hub.Clients.All.SendAsync("telemetryBroadcaster", context.Message, context.CancellationToken);

        _logger.LogInformation("New telemetry data pushed to hub");
    }
}

