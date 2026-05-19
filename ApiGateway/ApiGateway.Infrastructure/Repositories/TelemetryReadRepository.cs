using ApiGateway.Application.Abstractions;
using ApiGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Infrastructure.Repositories;

public class TelemetryReadRepository : ITelemetryReadRepository
{
    private readonly TelemetryReadDbContext _db;

    public TelemetryReadRepository(TelemetryReadDbContext db)
    {
        _db = db;
    }

    public IQueryable<Location> GetLocations() => _db.Locations.AsNoTracking();

    public IQueryable<EnergyMetric> GetEnergyMetrics() => _db.EnergyMetrics
        .AsNoTracking()
        .Include(x => x.Location);

    public IQueryable<MotionMetric> GetMotionMetrics() => _db.MotionMetrics
        .AsNoTracking()
        .Include(x => x.Location);

    public IQueryable<AirQualityMetric> GetAirQualityMetrics() => _db.AirQualityMetrics
        .AsNoTracking()
        .Include(x => x.Location);
}
