using ApiGateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Infrastructure;

public class TelemetryReadDbContext : DbContext
{
    public TelemetryReadDbContext(DbContextOptions<TelemetryReadDbContext> options) : base(options) { }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<EnergyMetric> EnergyMetrics => Set<EnergyMetric>();
    public DbSet<MotionMetric> MotionMetrics => Set<MotionMetric>();
    public DbSet<AirQualityMetric> AirQualityMetrics => Set<AirQualityMetric>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Location>().ToTable("locations");
        modelBuilder.Entity<EnergyMetric>().ToTable("energy_metrics");
        modelBuilder.Entity<MotionMetric>().ToTable("motion_metrics");
        modelBuilder.Entity<AirQualityMetric>().ToTable("air_quality_metrics");
    }
}
