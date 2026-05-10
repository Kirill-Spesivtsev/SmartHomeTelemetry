using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGateway.Domain.Entities;

public class BaseMetric
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public long LocationId { get; set; }
    public Location Location { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
