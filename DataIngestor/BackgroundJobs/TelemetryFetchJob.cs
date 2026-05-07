using DataIngestor.Configuration;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartHomeTelemetry.Shared.Contracts;
using SmartHomeTelemetry.Shared.Enums;
using System.Net.Http.Json;
using System.Text.Json;

namespace DataIngestor.BackgroundJobs;

public class TelemetryFetchJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly TelemetryFetchOptions _options;

    public TelemetryFetchJob(
        IHttpClientFactory httpClientFactory,
        IPublishEndpoint publishEndpoint,
        IBackgroundJobClient backgroundJobClient,
        IOptions<TelemetryFetchOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _publishEndpoint = publishEndpoint;
        _backgroundJobClient = backgroundJobClient;
        _options = options.Value;
    }

    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync()
    {
        var intervalSeconds = _options.IntervalSeconds;

        var httpClient = _httpClientFactory.CreateClient();

        var metrics = await httpClient.GetFromJsonAsync<List<UnstableMetricDto>>("/meters", new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (metrics is null || metrics.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var list = new List<TelemetryEvent>();

        foreach (var metric in metrics)
        {
            if (!TryMap(metric, now, out var evt))
            {
                continue;
            }

            list.Add(evt);
        }

        if (list.Count > 0)
        {
            await PublishBatchAsync(list, now);
        }

    }

    private static bool TryMap(UnstableMetricDto meter, DateTime timestampUtc, out TelemetryEvent evt)
    {
        evt = default!;

        var metricType = meter.Type?.Trim().ToLowerInvariant() switch
        {
            "energy" => MetricType.Energy,
            "motion" => MetricType.Motion,
            "air_quality" => MetricType.AirQuality,
            _ => MetricType.None
        };

        if (metricType == MetricType.None)
            return false;

        evt = new TelemetryEvent(
            EventId: Guid.NewGuid(),
            TimestampUtc: timestampUtc,
            MetricType: metricType,
            LocationName: meter.Name,
            Payload: meter.Payload
        );
        return true;
    }

    private Task PublishBatchAsync(List<TelemetryEvent> items, DateTime timestampUtc)
    {
        var batchEvent = new TelemetryBatchEvent(
            TimestampUtc: timestampUtc,
            Items: items.ToArray());

        return _publishEndpoint.Publish(batchEvent);
    }
}

