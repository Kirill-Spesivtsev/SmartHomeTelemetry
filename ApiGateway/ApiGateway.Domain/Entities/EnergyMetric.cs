using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGateway.Domain.Entities;

public class EnergyMetric : BaseMetric
{
    public float Energy { get; set; }
}
