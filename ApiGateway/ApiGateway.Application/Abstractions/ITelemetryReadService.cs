using ApiGateway.Application.Dtos;
using ApiGateway.Domain.Entities;

namespace ApiGateway.Application.Services;

public interface ITelemetryReadService
{
    public IQueryable<LocationDto> GetLocations();

    public IQueryable<EnergyMetric> GetEnergyMetrics();

    public IQueryable<MotionMetric> GetMotionMetrics();

    public IQueryable<AirQualityMetric> GetAirQualityMetrics();
}
