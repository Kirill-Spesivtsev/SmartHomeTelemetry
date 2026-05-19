using System;
using System.Collections.Generic;
using System.Text;

namespace DataProcessor.Domain.Entities;

public class MotionMetric : BaseMetric
{
    public bool MotionDetected { get; set; }
}
