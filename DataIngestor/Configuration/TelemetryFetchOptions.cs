namespace DataIngestor.Configuration;

public class TelemetryFetchOptions
{
    public const string SectionName = "TelemetryFetch";
    public int IntervalSeconds { get; set; } = 30;
    public int BatchSize { get; set; } = 20;
}
