using DataProcessor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataProcessor.Infrastructure;

public class TelemetryDbContext : DbContext
{
    public TelemetryDbContext(DbContextOptions<TelemetryDbContext> options) : base(options) { }

    public DbSet<Location> Locations => Set<Location>();
    public DbSet<EnergyMetric> EnergyMetrics => Set<EnergyMetric>();
    public DbSet<MotionMetric> MotionMetrics => Set<MotionMetric>();
    public DbSet<AirQualityMetric> AirQualityMetrics => Set<AirQualityMetric>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Location>(b =>
        {
            b.ToTable("locations");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).HasMaxLength(200).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<EnergyMetric>(b =>
        {
            b.ToTable("energy_metrics");
            b.HasKey(x => x.Id);
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.Energy).IsRequired();
            b.HasOne(x => x.Location).WithMany(x => x.EnergyMetrics).HasForeignKey(x => x.LocationId);
            b.HasIndex(x => new { x.LocationId, x.CreatedAt });
        });

        modelBuilder.Entity<MotionMetric>(b =>
        {
            b.ToTable("motion_metrics");
            b.HasKey(x => x.Id);
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.MotionDetected).IsRequired();
            b.HasOne(x => x.Location).WithMany(x => x.MotionMetrics).HasForeignKey(x => x.LocationId);
            b.HasIndex(x => new { x.LocationId, x.CreatedAt });
        });

        modelBuilder.Entity<AirQualityMetric>(b =>
        {
            b.ToTable("air_quality_metrics");
            b.HasKey(x => x.Id);
            b.Property(x => x.CreatedAt).IsRequired();
            b.Property(x => x.Co2).IsRequired();
            b.Property(x => x.Pm25).IsRequired();
            b.Property(x => x.Humidity).IsRequired();
            b.HasOne(x => x.Location).WithMany(x => x.AirQualityMetrics).HasForeignKey(x => x.LocationId);
            b.HasIndex(x => new { x.LocationId, x.CreatedAt });
        });

        modelBuilder.Entity<Location>().HasData(
            new Location { Id = 1, Name = "Bedroom" },
            new Location { Id = 2, Name = "Corridor" },
            new Location { Id = 3, Name = "Living Room" },
            new Location { Id = 4, Name = "Kitchen" },
            new Location { Id = 5, Name = "Office" },
            new Location { Id = 6, Name = "Garage" });
    }
}

