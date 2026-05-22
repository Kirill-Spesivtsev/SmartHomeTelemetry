using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Consumers;
using NotificationService.Hubs;
using SmartHomeTelemetry.Shared.Contracts;
using SmartHomeTelemetry.Shared.Enums;
using System.Text.Json;

namespace NotificationService.Tests;

public class TelemetryBroadcastConsumerTests
{
    private static TelemetryBroadcastConsumer CreateConsumer(Mock<IClientProxy> clientProxy)
    {
        var clients = new Mock<IHubClients>();
        clients.Setup(c => c.All).Returns(clientProxy.Object);

        var hubContext = new Mock<IHubContext<TelemetryHub>>();
        hubContext.Setup(h => h.Clients).Returns(clients.Object);

        return new TelemetryBroadcastConsumer(
            hubContext.Object,
            Mock.Of<ILogger<TelemetryBroadcastConsumer>>());
    }

    private static ConsumeContext<TelemetryBatchEvent> CreateConsumeContext(TelemetryBatchEvent message)
    {
        var mock = new Mock<ConsumeContext<TelemetryBatchEvent>>();
        mock.Setup(x => x.Message).Returns(message);
        mock.Setup(x => x.CancellationToken).Returns(CancellationToken.None);
        return mock.Object;
    }

    [Fact]
    public async Task Consume_SendsBatchToAllClients()
    {
        var clientProxy = new Mock<IClientProxy>();
        var consumer = CreateConsumer(clientProxy);
        var batch = new TelemetryBatchEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            [
                new TelemetryEvent(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    MetricType.Energy,
                    "Kitchen",
                    JsonDocument.Parse("""{"energy": 1.2}""").RootElement)
            ]);

        TelemetryBatchEvent? sentPayload = null;
        clientProxy
            .Setup(c => c.SendCoreAsync(
                "telemetryBroadcaster",
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, object?[], CancellationToken>((_, args, _) =>
                sentPayload = (TelemetryBatchEvent)args[0]!)
            .Returns(Task.CompletedTask);

        await consumer.Consume(CreateConsumeContext(batch));

        clientProxy.Verify(
            c => c.SendCoreAsync(
                "telemetryBroadcaster",
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        sentPayload.Should().BeEquivalentTo(batch);
    }

}
