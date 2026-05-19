using ApiGateway.Domain.Entities;

namespace ApiGateway.Application.Abstractions;

public interface ITelemetryReadRepository
{
    IQueryable<Location> GetLocations();
    IQueryable<EnergyMetric> GetEnergyMetrics();
    IQueryable<MotionMetric> GetMotionMetrics();
    IQueryable<AirQualityMetric> GetAirQualityMetrics();
}
