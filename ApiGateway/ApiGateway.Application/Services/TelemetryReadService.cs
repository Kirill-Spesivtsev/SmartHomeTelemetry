using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Dtos;
using ApiGateway.Domain.Constants;
using ApiGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace ApiGateway.Application.Services;

public class TelemetryReadService : ITelemetryReadService
{
    private readonly ITelemetryReadRepository _repository;

    public TelemetryReadService(ITelemetryReadRepository repository)
    {
        _repository = repository;
    }

    public IQueryable<LocationDto> GetLocations()
    {
        return _repository.GetLocations()
            .OrderBy(x => x.Name)
            .Select(x => new LocationDto(x.Id, x.Name));
    }

    public IQueryable<EnergyMetric> GetEnergyMetrics() =>
        _repository.GetEnergyMetrics();

    public IQueryable<MotionMetric> GetMotionMetrics() =>
        _repository.GetMotionMetrics();

    public IQueryable<AirQualityMetric> GetAirQualityMetrics() =>
        _repository.GetAirQualityMetrics();

    public async Task<IReadOnlyList<AirQualityMetricDto>> GetLatestAirQualityMetrics(
        DateTime? fromUtc,
        CancellationToken cancellationToken)
    {
        var query = _repository.GetAirQualityMetrics()
        .Where(m =>
            m.CreatedAt ==
            _repository.GetAirQualityMetrics()
                .Where(x => x.LocationId == m.LocationId)
                .Max(x => x.CreatedAt)
        );


        if (fromUtc.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= fromUtc.Value);
        }

        return await query
            .OrderBy(m => m.Location!.Name)
            .Select(m => new AirQualityMetricDto(
                m.LocationId,
                m.Location!.Name,
                MetricTypeNames.AirQuality,
                m.CreatedAt,
                m.Co2,
                m.Pm25,
                m.Humidity))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EnergyMetricDto>> GetLatestEnergyMetrics(
        DateTime? fromUtc,
        CancellationToken cancellationToken)
    {
        var query = _repository.GetEnergyMetrics()
            .Where(m =>
                m.CreatedAt ==
                _repository.GetEnergyMetrics()
                    .Where(x => x.LocationId == m.LocationId)
                    .Max(x => x.CreatedAt)
            );

        if (fromUtc.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= fromUtc.Value);
        }

        return await query
            .OrderBy(m => m.Location!.Name)
            .Select(m => new EnergyMetricDto(
                m.LocationId,
                m.Location!.Name,
                MetricTypeNames.Energy,
                m.CreatedAt,
                m.Energy))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<MotionMetricDto>> GetLatestMotionMetrics(
        DateTime? fromUtc,
        CancellationToken cancellationToken)
    {
        var metrics = _repository.GetMotionMetrics();

        var query = metrics
            .Where(m =>
                m.CreatedAt ==
                metrics
                    .Where(x => x.LocationId == m.LocationId)
                    .Max(x => x.CreatedAt)
            );

        if (fromUtc.HasValue)
        {
            query = query.Where(m => m.CreatedAt >= fromUtc.Value);
        }

        return await query
            .OrderBy(m => m.Location!.Name)
            .Select(m => new MotionMetricDto(
                m.LocationId,
                m.Location!.Name,
                MetricTypeNames.Motion,
                m.CreatedAt,
                m.MotionDetected))
            .ToListAsync(cancellationToken);
    }

}
