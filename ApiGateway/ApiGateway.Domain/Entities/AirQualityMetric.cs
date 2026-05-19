using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGateway.Domain.Entities;

public class AirQualityMetric : BaseMetric
{
    public int Co2 { get; set; }

    public int Pm25 { get; set; }

    public int Humidity { get; set; }
}
