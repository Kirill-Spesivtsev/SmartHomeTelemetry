using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGateway.Domain.Entities;

public class MotionMetric : BaseMetric
{
    public bool MotionDetected { get; set; }
}
