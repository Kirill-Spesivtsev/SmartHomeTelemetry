using DataIngestor.BackgroundJobs;
using DataIngestor.Configuration;
using Hangfire;

namespace DataIngestor.Helpers;

public static class JobHelper
{
    public static void SetUpRecurringFetch(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        var options = scope.ServiceProvider.GetRequiredService<
            Microsoft.Extensions.Options.IOptions<TelemetryFetchOptions>>().Value;

        var cronTemplate = $"*/{options.IntervalSeconds} * * * * *";

        recurringJobManager.AddOrUpdate<TelemetryFetchJob>(
            "telemetry-fetch",
            j => j.RunAsync(),
            cronTemplate
        );
    }
}
