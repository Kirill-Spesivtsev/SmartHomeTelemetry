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

}
