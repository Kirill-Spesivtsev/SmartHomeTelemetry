using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGateway.Application.Dtos;

public record AirQualityAggregateByLocation
{
    public long LocationId { get; init; }
    public string LocationName { get; init; } = default!;
    public int Count { get; init; }
    public double AvgCo2 { get; init; }
    public int MinCo2 { get; init; }
    public int MaxCo2 { get; init; }
    public double AvgPm25 { get; init; }
    public int MinPm25 { get; init; }
    public int MaxPm25 { get; init; }
    public double AvgHumidity { get; init; }
    public int MinHumidity { get; init; }
    public int MaxHumidity { get; init; }
}
