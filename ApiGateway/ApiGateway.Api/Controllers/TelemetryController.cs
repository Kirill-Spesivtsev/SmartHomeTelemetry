using ApiGateway.Application.Dtos;
using ApiGateway.Application.Services;
using ApiGateway.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Location = ApiGateway.Domain.Entities.Location;

namespace ApiGateway.Api.Controllers;

[ApiController]
[Route("api/telemetry")]
public class TelemetryController : ControllerBase
{
    private readonly TelemetryReadService _service;

    public TelemetryController(TelemetryReadService service)
    {
        _service = service;
    }

    [HttpGet("locations")]
    public async Task<ActionResult<IReadOnlyList<LocationDto>>> GetLocations(
    CancellationToken cancellationToken)
    {
        var result = await _service.GetLocations().ToListAsync();
        return Ok(result);
    }

    [HttpGet("energy")]
    public async Task<ActionResult<IReadOnlyList<EnergyMetric>>> GetEnergyMetrics(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetEnergyMetrics().ToListAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("motion")]
    public async Task<ActionResult<IReadOnlyList<MotionMetric>>> GetMotionMetrics(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetMotionMetrics().ToListAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("air-quality")]
    public async Task<ActionResult<IReadOnlyList<AirQualityMetric>>> GetAirQualityMetrics(
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAirQualityMetrics().ToListAsync(cancellationToken);
        return Ok(result);
    }

    [HttpGet("latest/air-quality")]
    public async Task<ActionResult<IReadOnlyList<AirQualityMetricDto>>> GetLatestAirQualityMetrics(
        [FromQuery] DateTime? fromUtc,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetLatestAirQualityMetrics(fromUtc, cancellationToken);
        return Ok(result);
    }

    [HttpGet("latest/energy")]
    public async Task<ActionResult<IReadOnlyList<EnergyMetricDto>>> GetLatestEnergyMetrics(
        [FromQuery] DateTime? fromUtc,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetLatestEnergyMetrics(fromUtc, cancellationToken);
        return Ok(result);
    }

    [HttpGet("latest/motion")]
    public async Task<ActionResult<IReadOnlyList<MotionMetricDto>>> GetLatestMotionMetrics(
        [FromQuery] DateTime? fromUtc,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetLatestMotionMetrics(fromUtc, cancellationToken);
        return Ok(result);
    }

    [HttpGet("aggregates/energy-by-location")]
    public async Task<ActionResult<IReadOnlyList<EnergyAggregateByLocation>>> GetEnergyAggregatesByLocation(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetEnergyAggregatesByLocation(fromUtc, toUtc, cancellationToken);
        return Ok(result);
    }

    [HttpGet("aggregates/air-quality-by-location")]
    public async Task<ActionResult<IReadOnlyList<AirQualityAggregateByLocation>>> GetAirQualityAggregatesByLocation(
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        var result = await _service.GetAirQualityAggregatesByLocation(fromUtc, toUtc, cancellationToken);
        return Ok(result);
    }
}

