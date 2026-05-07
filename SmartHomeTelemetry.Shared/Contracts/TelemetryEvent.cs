using System.Text.Json;
using SmartHomeTelemetry.Shared.Enums;

namespace SmartHomeTelemetry.Shared.Contracts;

public record TelemetryEvent(
    Guid EventId,
    DateTime TimestampUtc,
    MetricType MetricType,
    string LocationName,
    JsonElement Payload);

public record TelemetryBatchEvent(
    DateTime TimestampUtc,
    IReadOnlyList<TelemetryEvent> Items);

