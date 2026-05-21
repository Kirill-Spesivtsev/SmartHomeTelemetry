using System.Text.Json;
using DataProcessor.Domain.Entities;
using DataProcessor.Infrastructure;
using DataProcessor.Infrastructure.Messaging.Consumers;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SmartHomeTelemetry.Shared.Contracts;
using SmartHomeTelemetry.Shared.Enums;

namespace DataProcessor.Tests;

public class TelemetryEventConsumerTests
{
    private static TelemetryDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TelemetryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TelemetryDbContext(options);
    }

    private static TelemetryEvent CreateEvent(
        string locationName,
        MetricType metricType,
        string payloadJson,
        DateTime? timestampUtc = null)
    {
        return new(
            Guid.NewGuid(),
            timestampUtc ?? DateTime.UtcNow,
            metricType,
            locationName,
            JsonDocument.Parse(payloadJson).RootElement);
    }

    private static ConsumeContext<TelemetryBatchEvent> CreateConsumeContext(TelemetryBatchEvent message)
    {
        var mock = new Mock<ConsumeContext<TelemetryBatchEvent>>();
        mock.Setup(x => x.Message).Returns(message);
        mock.Setup(x => x.CancellationToken).Returns(CancellationToken.None);
        return mock.Object;
    }

    [Fact]
    public async Task Consume_WhenBatchHasEnergyMotionAndAirQuality_PersistsAllMetricTypes()
    {
        await using var db = CreateDbContext();
        var consumer = new TelemetryEventConsumer(db, Mock.Of<ILogger<TelemetryEventConsumer>>());

        var batch = new TelemetryBatchEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            [
                CreateEvent("NewLab", MetricType.Energy, """{"energy": 12.5}"""),
                CreateEvent("NewLab", MetricType.Motion, """{"motionDetected": true}"""),
                CreateEvent("NewLab", MetricType.AirQuality, """{"co2": 500, "pm25": 18, "humidity": 42}""")
            ]);

        await consumer.Consume(CreateConsumeContext(batch));

        (await db.Locations.SingleAsync(l => l.Name == "NewLab")).Should().NotBeNull();
        (await db.EnergyMetrics.CountAsync()).Should().Be(1);
        (await db.MotionMetrics.CountAsync()).Should().Be(1);
        (await db.AirQualityMetrics.CountAsync()).Should().Be(1);

        var energy = await db.EnergyMetrics.Include(m => m.Location).SingleAsync();
        energy.Energy.Should().Be(12.5f);
        energy.Location.Name.Should().Be("NewLab");
    }

    [Fact]
    public async Task Consume_WhenBatchIsEmpty_DoesNotPersistMetrics()
    {
        await using var db = CreateDbContext();
        var consumer = new TelemetryEventConsumer(db, Mock.Of<ILogger<TelemetryEventConsumer>>());

        var beforeEnergy = await db.EnergyMetrics.CountAsync();
        var beforeLocations = await db.Locations.CountAsync();

        await consumer.Consume(CreateConsumeContext(new TelemetryBatchEvent(Guid.NewGuid(), DateTime.UtcNow, [])));

        (await db.EnergyMetrics.CountAsync()).Should().Be(beforeEnergy);
        (await db.Locations.CountAsync()).Should().Be(beforeLocations);
    }

    [Fact]
    public async Task Consume_WhenLocationAlreadyExists_ReusesLocationWithoutDuplicating()
    {
        await using var db = CreateDbContext();
        db.Locations.Add(new Location { Id = 100, Name = "ExistingRoom" });
        await db.SaveChangesAsync();

        var consumer = new TelemetryEventConsumer(db, Mock.Of<ILogger<TelemetryEventConsumer>>());
        var batch = new TelemetryBatchEvent(
            Guid.NewGuid(),
            DateTime.UtcNow,
            [CreateEvent("ExistingRoom", MetricType.Energy, """{"energy": 3}""")]);

        await consumer.Consume(CreateConsumeContext(batch));

        (await db.Locations.CountAsync(l => l.Name == "ExistingRoom")).Should().Be(1);
        (await db.EnergyMetrics.CountAsync()).Should().Be(1);
    }
}
