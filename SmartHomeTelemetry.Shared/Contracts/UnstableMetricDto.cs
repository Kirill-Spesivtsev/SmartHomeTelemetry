using System.Text.Json;

namespace SmartHomeTelemetry.Shared.Contracts;

public record UnstableMetricDto(
    string Type,
    string Name,
    JsonElement Payload
);

