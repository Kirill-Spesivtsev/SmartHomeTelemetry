using DataProcessor.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using DataProcessor.Infrastructure.Helpers;
using SmartHomeTelemetry.Shared.Contracts;
using SmartHomeTelemetry.Shared.Enums;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace DataProcessor.Infrastructure.Messaging.Consumers;


public class TelemetryEventConsumer : IConsumer<TelemetryBatchEvent>
{
    private readonly TelemetryDbContext _db;
    private readonly ILogger<TelemetryEventConsumer> _logger;

    public TelemetryEventConsumer(TelemetryDbContext db, ILogger<TelemetryEventConsumer> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TelemetryBatchEvent> context)
    {
        var items = context.Message.Items;

        if (items.Count == 0)
        {
            return;
        }

        var names = items.Select(x => x.LocationName).Distinct().ToArray();
        var locations = await _db.Locations
            .Where(l => names.Contains(l.Name))
            .ToListAsync(context.CancellationToken);
        var nextId = locations.Count > 0 ? locations.Max(l => l.Id) + 1 : 1;
        var map = locations.ToDictionary(x => x.Name);

        foreach (var name in names.Where(n => !map.ContainsKey(n)))
        {
            var location = new Location { Name = name, Id = nextId };
            nextId++;
            _db.Locations.Add(location);
            map[name] = location;
        }

        foreach (var msg in items)
        {
            if (!map.TryGetValue(msg.LocationName, out var location))
            {
                continue;
            }

            switch (msg.MetricType)
            {
                case MetricType.Energy:
                    _db.EnergyMetrics.Add(PayloadParseHelper.GetEnergy(location.Id, msg.TimestampUtc, msg.Payload));
                    break;

                case MetricType.Motion:
                    _db.MotionMetrics.Add(PayloadParseHelper.GetMotion(location.Id, msg.TimestampUtc, msg.Payload));
                    break;

                case MetricType.AirQuality:
                    _db.AirQualityMetrics.Add(PayloadParseHelper.GetAirQuality(location.Id, msg.TimestampUtc, msg.Payload));
                    break;
            }
        }

        await _db.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Database was updated with the new data");
    }
}

