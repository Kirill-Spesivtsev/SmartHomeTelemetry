using ApiGateway.Application.Abstractions;
using ApiGateway.Application.Services;
using ApiGateway.Domain.Constants;
using ApiGateway.Domain.Entities;
using FluentAssertions;
using Moq;

namespace ApiGateway.Tests;

public class TelemetryReadServiceUnitTests
{
    private readonly Mock<ITelemetryReadRepository> _repoMock = new();

    private TelemetryReadService CreateService() => new TelemetryReadService(_repoMock.Object);

    [Fact]
    public void GetLocations_OrdersByName()
    {
        _repoMock.Setup(r => r.GetLocations())
            .Returns(new[]
            {
                new Location { Id = 2, Name = "Bedroom" },
                new Location { Id = 1, Name = "Kitchen" }
            }.AsQueryable());

        var service = CreateService();

        var result = service.GetLocations().ToList();

        result.Select(x => x.Name)
            .Should().Equal("Bedroom", "Kitchen");
    }

    [Fact]
    public void GetLatestEnergyMetrics_ReturnsLatestPerLocation()
    {
        var now = DateTime.UtcNow;

        var data = new[]
        {
            new EnergyMetric
            {
                LocationId = 1,
                Energy = 1,
                CreatedAt = now.AddMinutes(-10),
                Location = new Location { Name = "Office" }
            },
            new EnergyMetric
            {
                LocationId = 1,
                Energy = 5,
                CreatedAt = now,
                Location = new Location { Name = "Office" }
            }
        }.AsQueryable();

        _repoMock.Setup(r => r.GetEnergyMetrics()).Returns(data);

        var service = CreateService();

        var result = service.GetLatestEnergyMetrics(null).ToList();

        result.Should().ContainSingle();
        result[0].Energy.Should().Be(5);
    }

    [Fact]
    public void GetLatestEnergyMetrics_RespectsFromUtc()
    {
        var now = DateTime.UtcNow;

        var data = new[]
        {
            new EnergyMetric
            {
                LocationId = 1,
                Energy = 5,
                CreatedAt = now.AddHours(-2),
                Location = new Location { Name = "Office" }
            }
        }.AsQueryable();

        _repoMock.Setup(r => r.GetEnergyMetrics()).Returns(data);

        var service = CreateService();

        var result = service.GetLatestEnergyMetrics(now.AddHours(-1)).ToList();

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetLatestMotionMetrics_ReturnsLatest()
    {
        var now = DateTime.UtcNow;

        var data = new[]
        {
            new MotionMetric
            {
                LocationId = 1,
                MotionDetected = false,
                CreatedAt = now.AddMinutes(-5),
                Location = new Location { Name = "Garage" }
            },
            new MotionMetric
            {
                LocationId = 1,
                MotionDetected = true,
                CreatedAt = now,
                Location = new Location { Name = "Garage" }
            }
        }.AsQueryable();

        _repoMock.Setup(r => r.GetMotionMetrics()).Returns(data);

        var service = CreateService();

        var result = service.GetLatestMotionMetrics(null).ToList();

        result.Should().ContainSingle();
        result[0].MotionDetected.Should().BeTrue();
    }

    [Fact]
    public void GetLatestAirQualityMetrics_ReturnsLatestPerLocation()
    {
        var now = DateTime.UtcNow;

        var data = new[]
        {
            new AirQualityMetric
            {
                LocationId = 1,
                Co2 = 400,
                CreatedAt = now.AddMinutes(-10),
                Location = new Location { Name = "Kitchen" }
            },
            new AirQualityMetric
            {
                LocationId = 1,
                Co2 = 800,
                CreatedAt = now,
                Location = new Location { Name = "Kitchen" }
            }
        }.AsQueryable();

        _repoMock.Setup(r => r.GetAirQualityMetrics()).Returns(data);

        var service = CreateService();

        var result = service.GetLatestAirQualityMetrics(null).ToList();

        result.Should().ContainSingle();
        result[0].Co2.Should().Be(800);
    }

    [Fact]
    public void GetEnergyAggregatesByLocation_ComputesCorrectly()
    {
        var now = DateTime.UtcNow;

        var data = new[]
        {
            new EnergyMetric
            {
                LocationId = 1,
                Energy = 2,
                CreatedAt = now,
                Location = new Location { Name = "Office" }
            },
            new EnergyMetric
            {
                LocationId = 1,
                Energy = 4,
                CreatedAt = now,
                Location = new Location { Name = "Office" }
            }
        }.AsQueryable();

        _repoMock.Setup(r => r.GetEnergyMetrics()).Returns(data);

        var service = CreateService();

        var result = service.GetEnergyAggregatesByLocation(now.AddHours(-1), now).ToList();

        result.Should().ContainSingle();

        var agg = result[0];
        agg.Count.Should().Be(2);
        agg.AvgEnergy.Should().Be(3);
        agg.MinEnergy.Should().Be(2);
        agg.MaxEnergy.Should().Be(4);
    }

    [Fact]
    public void GetAirQualityAggregatesByLocation_ComputesCorrectly()
    {
        var now = DateTime.UtcNow;

        var data = new[]
        {
            new AirQualityMetric
            {
                LocationId = 1,
                Co2 = 400,
                Pm25 = 10,
                Humidity = 30,
                CreatedAt = now,
                Location = new Location { Name = "Kitchen" }
            },
            new AirQualityMetric
            {
                LocationId = 1,
                Co2 = 600,
                Pm25 = 20,
                Humidity = 50,
                CreatedAt = now,
                Location = new Location { Name = "Kitchen" }
            }
        }.AsQueryable();

        _repoMock.Setup(r => r.GetAirQualityMetrics()).Returns(data);

        var service = CreateService();

        var result = service.GetAirQualityAggregatesByLocation(now.AddHours(-1), now).ToList();

        var agg = result.Single();

        agg.AvgCo2.Should().Be(500);
        agg.MinCo2.Should().Be(400);
        agg.MaxCo2.Should().Be(600);

        agg.AvgPm25.Should().Be(15);
        agg.MinPm25.Should().Be(10);
        agg.MaxPm25.Should().Be(20);

        agg.AvgHumidity.Should().Be(40);
        agg.MinHumidity.Should().Be(30);
        agg.MaxHumidity.Should().Be(50);
    }
}