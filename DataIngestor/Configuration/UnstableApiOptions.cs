namespace DataIngestor.Configuration;

public class UnstableApiOptions
{
    public const string SectionName = "UnstableApi";
    public string BaseUrl { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
}
