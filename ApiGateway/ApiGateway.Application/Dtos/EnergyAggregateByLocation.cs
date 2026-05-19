using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGateway.Application.Dtos;

public record EnergyAggregateByLocation
{
    public long LocationId { get; init; }
    public string LocationName { get; init; } = default!;
    public int Count { get; init; }
    public float AvgEnergy { get; init; }
    public float MinEnergy { get; init; }
    public float MaxEnergy { get; init; }
}
