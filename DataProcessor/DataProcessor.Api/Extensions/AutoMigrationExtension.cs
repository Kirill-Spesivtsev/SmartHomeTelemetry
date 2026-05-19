using DataProcessor.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;

namespace DataProcessor.Api.Extensions;

public static class AutoMigrationExtension
{
    public static async Task ApplyMigrationsAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TelemetryDbContext>>();

        using var dbContext = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();

        try
        {
            dbContext.Database.Migrate();
        }
        catch (Exception ex) 
        {
            logger.LogError(ex, "An error occurred while applying migrations");
        }

    }
}
