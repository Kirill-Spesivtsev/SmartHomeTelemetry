using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGateway.Application.Dtos;

public record AirQualityMetricDto(
    long LocationId,
    string LocationName,
    string Type,
    DateTime CreatedAtUtc,
    int Co2,
    int Pm25,
    int Humidity);
