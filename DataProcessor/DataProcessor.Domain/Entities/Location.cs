using System;
using System.Collections.Generic;
using System.Text;

namespace DataProcessor.Domain.Entities;

public class Location
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public List<AirQualityMetric> AirQualityMetrics { get; set; } = [];

    public List<EnergyMetric> EnergyMetrics { get; set; } = [];

    public List<MotionMetric> MotionMetrics { get; set; } = [];
}
