using DataProcessor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace DataProcessor.Infrastructure.Helpers;

internal static class PayloadParseHelper
{
    public static AirQualityMetric GetAirQuality(long locationId, DateTime createdAt, JsonElement payload)
    {
        return new AirQualityMetric
        {
            LocationId = locationId,
            CreatedAt = createdAt,
            Co2 = payload.TryGetProperty("co2", out var co2) ? co2.GetInt32() : 0,
            Pm25 = payload.TryGetProperty("pm25", out var pm25) ? pm25.GetInt32() : 0,
            Humidity = payload.TryGetProperty("humidity", out var humidity) ? humidity.GetInt32() : 0
        };
    }
    public static MotionMetric GetMotion(long locationId, DateTime createdAt, JsonElement payload)
    {
        return new MotionMetric
        {
            LocationId = locationId,
            CreatedAt = createdAt,
            MotionDetected = payload.TryGetProperty("motionDetected", out var motion) && motion.ValueKind == JsonValueKind.True
        };
    }

    public static EnergyMetric GetEnergy(long locationId, DateTime createdAt, JsonElement payload)
    {
        return new EnergyMetric
        {
            LocationId = locationId,
            CreatedAt = createdAt,
            Energy = payload.TryGetProperty("energy", out var energy) ? energy.GetSingle() : 0f
        };
    }
}
