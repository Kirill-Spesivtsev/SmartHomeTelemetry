using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ApiGateway.Domain.Entities;

public class Location
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public List<AirQualityMetric> AirQualityMetrics { get; set; } = new();

    [JsonIgnore]
    public List<EnergyMetric> EnergyMetrics { get; set; } = new();

    [JsonIgnore]
    public List<MotionMetric> MotionMetrics { get; set; } = new();
}
