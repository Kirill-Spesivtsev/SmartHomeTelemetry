using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Dtos;
using ApiGateway.Application.Services;
using ApiGateway.Domain.Entities;
using HotChocolate.CostAnalysis.Types;
using Microsoft.EntityFrameworkCore;
using Location = ApiGateway.Domain.Entities.Location;

namespace ApiGateway.Api.GraphQL;

public class Query
{
    [Cost(1)]
    [UsePaging(MaxPageSize = 20)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<LocationDto> GetLocations([Service] TelemetryReadService service) =>
        service.GetLocations();

    [Cost(2)]
    [UsePaging(MaxPageSize = 500)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<EnergyMetric> GetEnergyMetrics([Service] TelemetryReadService service) =>
        service.GetEnergyMetrics();

    [Cost(2)]
    [UsePaging(MaxPageSize = 500)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<MotionMetric> GetMotionMetrics([Service] TelemetryReadService service) =>
        service.GetMotionMetrics();

    [Cost(2)]
    [UsePaging(MaxPageSize = 500)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<AirQualityMetric> GetAirQualityMetrics([Service] TelemetryReadService service) =>
        service.GetAirQualityMetrics();


    [Cost(10)]
    [ListSize(AssumedSize = 20, RequireOneSlicingArgument = false)]
    public async Task<List<AirQualityMetricDto>> GetLatestAirQualityMetrics(
        [Service] TelemetryReadService service,
        DateTime? fromUtc,
        CancellationToken cancellationToken)
    {
        return await service.GetLatestAirQualityMetrics(fromUtc).ToListAsync(cancellationToken);

    }

    [Cost(10)]
    [ListSize(AssumedSize = 20, RequireOneSlicingArgument = false)]
    public async Task<List<EnergyMetricDto>> GetLatestEnergyMetrics(
        [Service] TelemetryReadService service, 
        DateTime? fromUtc, 
        CancellationToken cancellationToken)
    {
        return await service.GetLatestEnergyMetrics(fromUtc).ToListAsync(cancellationToken);
    }
        

    [Cost(10)]
    [ListSize(AssumedSize = 20, RequireOneSlicingArgument = false)]
    public async Task<List<MotionMetricDto>> GetLatestMotionMetrics(
        [Service] TelemetryReadService service, 
        DateTime? fromUtc, 
        CancellationToken cancellationToken)
    {
        return await service.GetLatestMotionMetrics(fromUtc).ToListAsync(cancellationToken);
    }
        

    [Cost(15)]
    [ListSize(AssumedSize = 20, RequireOneSlicingArgument = false)]
    public async Task<List<EnergyAggregateByLocation>> GetEnergyAggregatesByLocation(
        [Service] TelemetryReadService service, 
        DateTime? fromUtc, 
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        return await service.GetEnergyAggregatesByLocation(fromUtc, toUtc).ToListAsync(cancellationToken);
    }
        

    [Cost(15)]
    [ListSize(AssumedSize = 20, RequireOneSlicingArgument = false)]
    public async Task<List<AirQualityAggregateByLocation>> GetAirQualityAggregatesByLocation(
        [Service] TelemetryReadService service,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        return await service.GetAirQualityAggregatesByLocation(fromUtc, toUtc).ToListAsync(cancellationToken);
    }

}
