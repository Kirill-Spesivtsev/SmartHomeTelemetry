using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Hubs;

public class TelemetryHub : Hub
{
    public const string HubPath = "/hubs/telemetry";
}

