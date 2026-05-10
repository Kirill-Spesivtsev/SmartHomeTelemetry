
using ApiGateway.Application.Dtos;
using ApiGateway.Domain.Entities;

namespace ApiGateway.Application.Services;

public interface ITelemetryReadService
{
    public IQueryable<LocationDto> GetLocations();

    public IQueryable<EnergyMetric> GetEnergyMetrics();

    public IQueryable<MotionMetric> GetMotionMetrics();

    public IQueryable<AirQualityMetric> GetAirQualityMetrics();

    public Task<IReadOnlyList<AirQualityMetricDto>> GetLatestAirQualityMetrics(
        DateTime? fromUtc,
        CancellationToken cancellationToken);

    public Task<IReadOnlyList<EnergyMetricDto>> GetLatestEnergyMetrics(
        DateTime? fromUtc,
        CancellationToken cancellationToken);

    public Task<IReadOnlyList<MotionMetricDto>> GetLatestMotionMetrics(
        DateTime? fromUtc,
        CancellationToken cancellationToken);


    public Task<IReadOnlyList<EnergyAggregateByLocation>> GetEnergyAggregatesByLocation(
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken);

    public Task<IReadOnlyList<AirQualityAggregateByLocation>> GetAirQualityAggregatesByLocation(
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken);
}
