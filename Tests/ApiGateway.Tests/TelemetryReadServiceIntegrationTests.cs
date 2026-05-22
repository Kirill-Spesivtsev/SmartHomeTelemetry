using ApiGateway.Application.Dtos;
using ApiGateway.Application.Services;
using ApiGateway.Domain.Constants;
using ApiGateway.Domain.Entities;
using ApiGateway.Infrastructure;
using ApiGateway.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Tests;

public class TelemetryReadServiceIntegrationTests
{
    private static TelemetryReadService CreateService(Action<TelemetryReadDbContext>? data = null)
    {
        var options = new DbContextOptionsBuilder<TelemetryReadDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var db = new TelemetryReadDbContext(options);
        data?.Invoke(db);
        db.SaveChanges();

        return new TelemetryReadService(new TelemetryReadRepository(db));
    }

    private static Location CreateLocation(long id, string name) =>
        new() { Id = id, Name = name };

    [Fact]
    public void GetLocations_ReturnsLocationsOrderedByName()
    {
        var service = CreateService(db =>
        {
            db.Locations.AddRange(
                CreateLocation(1, "Kitchen"),
                CreateLocation(2, "Bedroom"));
        });

        var result = service.GetLocations().ToList();

        result.Should().HaveCount(2);
        result.Select(x => x.Name).Should().Equal("Bedroom", "Kitchen");
        result[0].Id.Should().Be(2);
    }

    [Fact]
    public void GetLocations_WhenDatabaseIsEmpty_ReturnsEmptyQueryable()
    {
        var service = CreateService();

        service.GetLocations().ToList().Should().BeEmpty();
    }

    [Fact]
    public void GetEnergyMetrics_ReturnsAllMetrics()
    {
        var location = CreateLocation(1, "Office");
        var service = CreateService(db =>
        {
            db.Locations.Add(location);
            db.EnergyMetrics.Add(new EnergyMetric
            {
                LocationId = 1,
                Location = location,
                Energy = 10f,
                CreatedAt = DateTime.UtcNow
            });
        });

        service.GetEnergyMetrics().ToList().Should().ContainSingle(m => m.Energy == 10f);
    }

    [Fact]
    public void GetEnergyMetrics_WhenNoMetrics_ReturnsEmptyQueryable()
    {
        var service = CreateService(db => db.Locations.Add(CreateLocation(1, "Office")));

        service.GetEnergyMetrics().ToList().Should().BeEmpty();
    }

    [Fact]
    public void GetMotionMetrics_ReturnsAllMetrics()
    {
        var location = CreateLocation(1, "Corridor");
        var service = CreateService(db =>
        {
            db.Locations.Add(location);
            db.MotionMetrics.Add(new MotionMetric
            {
                LocationId = 1,
                Location = location,
                MotionDetected = true,
                CreatedAt = DateTime.UtcNow
            });
        });

        service.GetMotionMetrics().ToList().Should().ContainSingle(m => m.MotionDetected);
    }

    [Fact]
    public void GetMotionMetrics_WhenNoMetrics_ReturnsEmptyQueryable()
    {
        var service = CreateService();

        service.GetMotionMetrics().ToList().Should().BeEmpty();
    }

    [Fact]
    public void GetAirQualityMetrics_ReturnsAllMetrics()
    {
        var location = CreateLocation(1, "Living Room");
        var service = CreateService(db =>
        {
            db.Locations.Add(location);
            db.AirQualityMetrics.Add(new AirQualityMetric
            {
                LocationId = 1,
                Location = location,
                Co2 = 400,
                Pm25 = 12,
                Humidity = 45,
                CreatedAt = DateTime.UtcNow
            });
        });

        service.GetAirQualityMetrics().ToList().Should().ContainSingle(m => m.Co2 == 400);
    }

    [Fact]
    public async Task GetLatestAirQualityMetrics_ReturnsLatestPerLocation()
    {
        var older = new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);
        var newer = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);
        var kitchen = CreateLocation(1, "Kitchen");
        var bedroom = CreateLocation(2, "Bedroom");

        var service = CreateService(db =>
        {
            db.Locations.AddRange(kitchen, bedroom);
            db.AirQualityMetrics.AddRange(
                new AirQualityMetric { LocationId = 1, Location = kitchen, Co2 = 300, Pm25 = 5, Humidity = 40, CreatedAt = older },
                new AirQualityMetric { LocationId = 1, Location = kitchen, Co2 = 500, Pm25 = 15, Humidity = 50, CreatedAt = newer },
                new AirQualityMetric { LocationId = 2, Location = bedroom, Co2 = 600, Pm25 = 20, Humidity = 55, CreatedAt = newer });
        });

        var result = service.GetLatestAirQualityMetrics(null).ToList();

        result.Should().HaveCount(2);
        result.Should().Contain(x => x.LocationName == "Kitchen" && x.Co2 == 500);
        result.Should().Contain(x => x.LocationName == "Bedroom" && x.Co2 == 600);
        result.Should().OnlyContain(x => x.Type == MetricTypeNames.AirQuality);
    }

    [Fact]
    public async Task GetLatestAirQualityMetrics_WhenFromUtcExcludesAll_ReturnsEmpty()
    {
        var kitchen = CreateLocation(1, "Kitchen");
        var createdAt = new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);

        var service = CreateService(db =>
        {
            db.Locations.Add(kitchen);
            db.AirQualityMetrics.Add(new AirQualityMetric
            {
                LocationId = 1,
                Location = kitchen,
                Co2 = 400,
                Pm25 = 10,
                Humidity = 40,
                CreatedAt = createdAt
            });
        });

        var result = service.GetLatestAirQualityMetrics(createdAt.AddHours(1)).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLatestEnergyMetrics_ReturnsLatestPerLocation()
    {
        var older = new DateTime(2026, 5, 1, 8, 0, 0, DateTimeKind.Utc);
        var newer = new DateTime(2026, 5, 1, 9, 0, 0, DateTimeKind.Utc);
        var office = CreateLocation(1, "Office");

        var service = CreateService(db =>
        {
            db.Locations.Add(office);
            db.EnergyMetrics.AddRange(
                new EnergyMetric { LocationId = 1, Location = office, Energy = 1f, CreatedAt = older },
                new EnergyMetric { LocationId = 1, Location = office, Energy = 9f, CreatedAt = newer });
        });

        var result = service.GetLatestEnergyMetrics(null).ToList();

        result.Should().ContainSingle()
            .Which.Should().Match<EnergyMetricDto>(x =>
                x.LocationName == "Office" &&
                x.Energy == 9f &&
                x.Type == MetricTypeNames.Energy);
    }

    [Fact]
    public async Task GetLatestMotionMetrics_ReturnsLatestPerLocation()
    {
        var older = new DateTime(2026, 5, 1, 8, 0, 0, DateTimeKind.Utc);
        var newer = new DateTime(2026, 5, 1, 9, 0, 0, DateTimeKind.Utc);
        var garage = CreateLocation(1, "Garage");

        var service = CreateService(db =>
        {
            db.Locations.Add(garage);
            db.MotionMetrics.AddRange(
                new MotionMetric { LocationId = 1, Location = garage, MotionDetected = false, CreatedAt = older },
                new MotionMetric { LocationId = 1, Location = garage, MotionDetected = true, CreatedAt = newer });
        });

        var result = service.GetLatestMotionMetrics(null).ToList();

        result.Should().ContainSingle()
            .Which.MotionDetected.Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestMotionMetrics_WhenFromUtcExcludesAll_ReturnsEmpty()
    {
        var garage = CreateLocation(1, "Garage");
        var createdAt = new DateTime(2026, 5, 1, 9, 0, 0, DateTimeKind.Utc);

        var service = CreateService(db =>
        {
            db.Locations.Add(garage);
            db.MotionMetrics.Add(new MotionMetric
            {
                LocationId = 1,
                Location = garage,
                MotionDetected = true,
                CreatedAt = createdAt
            });
        });

        var result = service.GetLatestMotionMetrics(createdAt.AddMinutes(1)).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetEnergyAggregatesByLocation_ComputesAggregatesInRange()
    {
        var from = new DateTime(2026, 5, 10, 0, 0, 0, DateTimeKind.Utc);
        var office = CreateLocation(1, "Office");

        var service = CreateService(db =>
        {
            db.Locations.Add(office);
            db.EnergyMetrics.AddRange(
                new EnergyMetric { LocationId = 1, Location = office, Energy = 2f, CreatedAt = from.AddHours(1) },
                new EnergyMetric { LocationId = 1, Location = office, Energy = 4f, CreatedAt = from.AddHours(2) },
                new EnergyMetric { LocationId = 1, Location = office, Energy = 3f, CreatedAt = from.AddDays(-2) });
        });

        var result = service.GetEnergyAggregatesByLocation(from, from.AddHours(3)).ToList();

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new
            {
                LocationName = "Office",
                Count = 2,
                AvgEnergy = 3f,
                MinEnergy = 2f,
                MaxEnergy = 4f
            });
    }

    [Fact]
    public async Task GetEnergyAggregatesByLocation_WhenToBeforeFrom_ReturnsEmpty()
    {
        var from = new DateTime(2026, 5, 10, 12, 0, 0, DateTimeKind.Utc);
        var to = from.AddHours(-1);
        var office = CreateLocation(1, "Office");

        var service = CreateService(db =>
        {
            db.Locations.Add(office);
            db.EnergyMetrics.Add(new EnergyMetric
            {
                LocationId = 1,
                Location = office,
                Energy = 5f,
                CreatedAt = from
            });
        });

        var result = service.GetEnergyAggregatesByLocation(from, to).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAirQualityAggregatesByLocation_ComputesAggregatesInRange()
    {
        var from = new DateTime(2026, 5, 10, 0, 0, 0, DateTimeKind.Utc);
        var kitchen = CreateLocation(1, "Kitchen");

        var service = CreateService(db =>
        {
            db.Locations.Add(kitchen);
            db.AirQualityMetrics.AddRange(
                new AirQualityMetric { LocationId = 1, Location = kitchen, Co2 = 400, Pm25 = 10, Humidity = 30, CreatedAt = from.AddHours(1) },
                new AirQualityMetric { LocationId = 1, Location = kitchen, Co2 = 600, Pm25 = 20, Humidity = 50, CreatedAt = from.AddHours(2) });
        });

        var result = service.GetAirQualityAggregatesByLocation(from, from.AddHours(3)).ToList();

        result.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new
            {
                LocationName = "Kitchen",
                Count = 2,
                AvgCo2 = 500,
                MinCo2 = 400,
                MaxCo2 = 600,
                AvgPm25 = 15,
                MinPm25 = 10,
                MaxPm25 = 20,
                AvgHumidity = 40,
                MinHumidity = 30,
                MaxHumidity = 50
            });
    }

    [Fact]
    public async Task GetAirQualityAggregatesByLocation_WhenNoMetricsInRange_ReturnsEmpty()
    {
        var from = new DateTime(2026, 5, 10, 0, 0, 0, DateTimeKind.Utc);
        var kitchen = CreateLocation(1, "Kitchen");

        var service = CreateService(db =>
        {
            db.Locations.Add(kitchen);
            db.AirQualityMetrics.Add(new AirQualityMetric
            {
                LocationId = 1,
                Location = kitchen,
                Co2 = 400,
                Pm25 = 10,
                Humidity = 30,
                CreatedAt = from.AddDays(-10)
            });
        });

        var result = service.GetAirQualityAggregatesByLocation(from, from.AddHours(1)).ToList();

        result.Should().BeEmpty();
    }

}
