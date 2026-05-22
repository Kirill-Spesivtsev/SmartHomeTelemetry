using System.Net;
using System.Text;
using System.Text.Json;
using DataIngestor.BackgroundJobs;
using DataIngestor.Configuration;
using FluentAssertions;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SmartHomeTelemetry.Shared.Contracts;

namespace DataIngestor.Tests;

public class TelemetryFetchJobTests
{
    private class FakeHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            handler(request);
    }

    private static TelemetryFetchJob CreateJob(
        HttpMessageHandler handler,
        Mock<IPublishEndpoint>? publishMock = null,
        int batchSize = 20)
    {
        publishMock ??= new Mock<IPublishEndpoint>(MockBehavior.Strict);

        var factory = new Mock<IHttpClientFactory>();
        factory.Setup(f => f.CreateClient("unstable-api"))
            .Returns(new HttpClient(handler) { BaseAddress = new Uri("http://unstable-api") });

        var options = Options.Create(new TelemetryFetchOptions { BatchSize = batchSize, IntervalSeconds = 30 });

        return new TelemetryFetchJob(
            factory.Object,
            publishMock.Object,
            Mock.Of<IBackgroundJobClient>(),
            options,
            Mock.Of<ILogger<TelemetryFetchJob>>());
    }

    private static Task<HttpResponseMessage> JsonResponse(string json) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        });

    [Fact]
    public async Task RunAsync_WhenApiReturnsValidMetrics_PublishesBatches()
    {
        const string json = """
            [
              {"type":"energy","name":"Kitchen","payload":{"energy":1.5}},
              {"type":"motion","name":"Hall","payload":{"motionDetected":true}}
            ]
            """;

        var handler = new FakeHttpMessageHandler(_ => JsonResponse(json));
        var publishMock = new Mock<IPublishEndpoint>();
        publishMock.Setup(p => p.Publish(It.IsAny<TelemetryBatchEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var job = CreateJob(handler, publishMock);

        await job.RunAsync();

        publishMock.Verify(
            p => p.Publish(
                It.Is<TelemetryBatchEvent>(b => b.Items.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RunAsync_WhenApiReturnsEmptyArray_DoesNotPublish()
    {
        var handler = new FakeHttpMessageHandler(_ => JsonResponse("[]"));
        var publishMock = new Mock<IPublishEndpoint>(MockBehavior.Strict);

        await CreateJob(handler, publishMock).RunAsync();

        publishMock.Verify(
            p => p.Publish(It.IsAny<TelemetryBatchEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RunAsync_WhenApiFails_DoesNotPublish()
    {
        var handler = new FakeHttpMessageHandler(_ =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));
        var publishMock = new Mock<IPublishEndpoint>(MockBehavior.Strict);

        await CreateJob(handler, publishMock).RunAsync();

        publishMock.Verify(
            p => p.Publish(It.IsAny<TelemetryBatchEvent>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RunAsync_WhenMetricsContainUnsupportedTypes_SkipsThemAndPublishesSupportedOnes()
    {
        const string json = """
            [
              {"type":"unknown","name":"Attic","payload":{"value":1}},
              {"type":"air_quality","name":"Bedroom","payload":{"co2":400,"pm25":10,"humidity":45}}
            ]
            """;

        var handler = new FakeHttpMessageHandler(_ => JsonResponse(json));
        var publishMock = new Mock<IPublishEndpoint>();
        publishMock.Setup(p => p.Publish(It.IsAny<TelemetryBatchEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await CreateJob(handler, publishMock).RunAsync();

        publishMock.Verify(
            p => p.Publish(
                It.Is<TelemetryBatchEvent>(b =>
                    b.Items.Count == 1 &&
                    b.Items[0].LocationName == "Bedroom"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RunAsync_WhenMetricsExceedBatchSize_PublishesMultipleBatches()
    {
        const string json = """
            [
              {"type":"energy","name":"Room0","payload":{"energy":0}},
              {"type":"energy","name":"Room1","payload":{"energy":1}},
              {"type":"energy","name":"Room2","payload":{"energy":2}}
            ]
            """;

        var handler = new FakeHttpMessageHandler(_ => JsonResponse(json));
        var publishMock = new Mock<IPublishEndpoint>();
        publishMock.Setup(p => p.Publish(It.IsAny<TelemetryBatchEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await CreateJob(handler, publishMock, batchSize: 2).RunAsync();

        publishMock.Verify(
            p => p.Publish(It.IsAny<TelemetryBatchEvent>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}
