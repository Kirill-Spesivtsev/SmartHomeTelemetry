

using DataIngestor.Configuration;
using DataIngestor.Helpers;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using MassTransit;
using MassTransit.RetryPolicies;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<UnstableApiOptions>(builder.Configuration.GetSection(UnstableApiOptions.SectionName));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.Configure<TelemetryFetchOptions>(builder.Configuration.GetSection(TelemetryFetchOptions.SectionName));

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console();
});

builder.Services.AddHttpClient("unstable-api", (sp, http) =>
{
    var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<UnstableApiOptions>>().Value;
    http.BaseAddress = new Uri(options.BaseUrl);
    http.DefaultRequestHeaders.Add("X-Api-Key", options.ApiKey);
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHangfire(config =>
{
    config.UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage();
});
builder.Services.AddHangfireServer();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        var rabbitOptions = ctx.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
        cfg.Host(rabbitOptions.Host, "/", h =>
        {
            h.Username(rabbitOptions.Username);
            h.Password(rabbitOptions.Password);
        });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();


app.UseHangfireDashboard("/hangfire");

JobHelper.SetUpRecurringFetch(app);

app.Run();

