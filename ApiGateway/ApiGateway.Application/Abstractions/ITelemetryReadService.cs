
using ApiGateway.Application.Dtos;
using ApiGateway.Domain.Constants;
using ApiGateway.Domain.Entities;

namespace ApiGateway.Application.Services;

public interface ITelemetryReadService
{
    public IQueryable<LocationDto> GetLocations();

    public IQueryable<EnergyMetric> GetEnergyMetrics();

    public IQueryable<MotionMetric> GetMotionMetrics();

    public IQueryable<AirQualityMetric> GetAirQualityMetrics();

    public IQueryable<AirQualityMetricDto> GetLatestAirQualityMetrics(DateTime? fromUtc);

    public IQueryable<EnergyMetricDto> GetLatestEnergyMetrics(DateTime? fromUtc);

    public IQueryable<MotionMetricDto> GetLatestMotionMetrics(DateTime? fromUtc);

    public IQueryable<EnergyAggregateByLocation> GetEnergyAggregatesByLocation(DateTime? fromUtc, DateTime? toUtc);

    public IQueryable<AirQualityAggregateByLocation> GetAirQualityAggregatesByLocation(DateTime? fromUtc, DateTime? toUtc);
}
