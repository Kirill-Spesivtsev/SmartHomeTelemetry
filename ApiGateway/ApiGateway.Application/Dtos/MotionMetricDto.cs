using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGateway.Application.Dtos;

public record MotionMetricDto(
    long LocationId,
    string LocationName,
    string Type,
    DateTime CreatedAtUtc,
    bool MotionDetected);
