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
    private readonly ILogger<TelemetryFetchJob> _logger;

    public TelemetryFetchJob(
        IHttpClientFactory httpClientFactory,
        IPublishEndpoint publishEndpoint,
        IBackgroundJobClient backgroundJobClient,
        IOptions<TelemetryFetchOptions> options,
        ILogger<TelemetryFetchJob> logger)
    {
        _httpClientFactory = httpClientFactory;
        _publishEndpoint = publishEndpoint;
        _backgroundJobClient = backgroundJobClient;
        _options = options.Value;
        _logger = logger;
    }

    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    [AutomaticRetry(Attempts = 0)]
    public async Task RunAsync()
    {
        var intervalSeconds = _options.IntervalSeconds;

        try
        {
            var httpClient = _httpClientFactory.CreateClient("unstable-api");

            var metrics = await httpClient.GetFromJsonAsync<List<UnstableMetricDto>>("/meters", new JsonSerializerOptions(JsonSerializerDefaults.Web));

            if (metrics is null || metrics.Count == 0)
            {
                _logger.LogWarning("Failed to fetch telemetry from unstable API.");
                return;
            }

            var now = DateTime.UtcNow;
            var published = 0;
            var batch = new List<TelemetryEvent>(_options.BatchSize);

            foreach (var metric in metrics)
            {
                if (!TryMap(metric, now, out var evt))
                {
                    _logger.LogWarning("Unsupported metric payload. type={Type} name={Name}", metric.Type, metric.Name);
                    continue;
                }

                batch.Add(evt);
                if (batch.Count >= _options.BatchSize)
                {
                    await PublishBatchAsync(batch, now);
                    published += batch.Count;
                    batch.Clear();
                }
            }

            if (batch.Count > 0)
            {
                await PublishBatchAsync(batch, now);
                published += batch.Count;
            }

            _logger.LogInformation("Published {Count} telemetry events in batches.", published);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Auto fetch job failed.");
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
            BatchId: Guid.NewGuid(),
            TimestampUtc: timestampUtc,
            Items: items.ToArray());

        return _publishEndpoint.Publish(batchEvent);
    }
}

