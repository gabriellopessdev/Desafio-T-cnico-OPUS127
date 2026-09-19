using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Opus127.Dengue.Api.Entities;
using Opus127.Dengue.Api.Integrations.AlertaDengue;
using Opus127.Dengue.Api.Repositories;
using Opus127.Dengue.Api.Services;

namespace Opus127.Dengue.Api.Tests;

public class DengueSyncServiceTests
{
    [Fact]
    public async Task SyncLastSixMonthsAsync_upserts_mapped_week_from_client()
    {
        var repository = new FakeDengueAlertRepository();
        var client = CreateClient(Week(202340, 45, 38, 2));
        var service = CreateService(client, repository);

        var upserted = await service.SyncLastSixMonthsAsync(CancellationToken.None);

        upserted.Should().BeGreaterThan(0);
        var stored = repository.Items.Should().ContainSingle().Subject;
        stored.EpidemiologicalYear.Should().Be(2023);
        stored.EpidemiologicalWeek.Should().Be(40);
        stored.EstimatedCases.Should().Be(45);
        stored.NotifiedCases.Should().Be(38);
        stored.AlertLevel.Should().Be(2);
        stored.Geocode.Should().Be("3106200");
    }

    [Fact]
    public async Task SyncLastSixMonthsAsync_updates_existing_week_on_second_call()
    {
        var repository = new FakeDengueAlertRepository();
        var client = CreateClient(Week(202340, 45, 38, 2));
        var service = CreateService(client, repository);

        await service.SyncLastSixMonthsAsync(CancellationToken.None);
        client.Weeks = [Week(202340, 45, 40, 2)];
        await service.SyncLastSixMonthsAsync(CancellationToken.None);

        var stored = repository.Items.Should().ContainSingle().Subject;
        stored.NotifiedCases.Should().Be(40);
        stored.EstimatedCases.Should().Be(45);
    }

    [Fact]
    public async Task SyncLastSixMonthsAsync_propagates_client_http_failure()
    {
        var repository = new FakeDengueAlertRepository();
        var client = Substitute.For<IAlertaDengueClient>();
        client.GetCityAlertsAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("origem indisponível"));
        var service = CreateService(client, repository);

        var act = () => service.SyncLastSixMonthsAsync(CancellationToken.None);

        await act.Should().ThrowAsync<HttpRequestException>();
        repository.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task HostedService_swallows_http_failure_and_logs()
    {
        var sync = Substitute.For<IDengueSyncService>();
        sync.SyncLastSixMonthsAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("origem indisponível"));

        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IDengueSyncService)).Returns(sync);
        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(provider);
        var scopeFactory = Substitute.For<IServiceScopeFactory>();
        scopeFactory.CreateScope().Returns(scope);
        var logger = Substitute.For<ILogger<DengueSyncHostedService>>();

        var hosted = new DengueSyncHostedService(scopeFactory, logger);
        var start = () => hosted.StartAsync(CancellationToken.None);

        await start.Should().NotThrowAsync();
        await hosted.StopAsync(CancellationToken.None);
        logger.Received().Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<HttpRequestException>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    private static DengueSyncService CreateService(IAlertaDengueClient client, IDengueAlertRepository repository)
    {
        var configuration = Substitute.For<IConfiguration>();
        configuration["AlertaDengue:Geocode"].Returns("3106200");
        return new DengueSyncService(client, repository, configuration);
    }

    private static ConfigurableAlertaDengueClient CreateClient(AlertaDengueWeekDto week)
        => new() { Weeks = [week] };

    private static AlertaDengueWeekDto Week(int se, decimal casosEst, int casos, int nivel) => new()
    {
        SE = se,
        casos_est = casosEst,
        casos = casos,
        nivel = nivel
    };

    private sealed class ConfigurableAlertaDengueClient : IAlertaDengueClient
    {
        public IReadOnlyList<AlertaDengueWeekDto> Weeks { get; set; } = [];

        public Task<IReadOnlyList<AlertaDengueWeekDto>> GetCityAlertsAsync(
            int ewStart, int ewEnd, int eyStart, int eyEnd, CancellationToken cancellationToken)
            => Task.FromResult(Weeks);
    }

    private sealed class FakeDengueAlertRepository : IDengueAlertRepository
    {
        public List<DengueWeeklyAlert> Items { get; } = [];

        public Task<DengueWeeklyAlert?> GetAsync(int year, int week, CancellationToken cancellationToken)
            => Task.FromResult(Items.FirstOrDefault(item =>
                item.EpidemiologicalYear == year && item.EpidemiologicalWeek == week));

        public Task UpsertRangeAsync(IEnumerable<DengueWeeklyAlert> alerts, CancellationToken cancellationToken)
        {
            foreach (var alert in alerts)
            {
                var existing = Items.FirstOrDefault(item =>
                    item.EpidemiologicalYear == alert.EpidemiologicalYear &&
                    item.EpidemiologicalWeek == alert.EpidemiologicalWeek);
                if (existing is null)
                {
                    Items.Add(alert);
                    continue;
                }

                existing.EstimatedCases = alert.EstimatedCases;
                existing.NotifiedCases = alert.NotifiedCases;
                existing.AlertLevel = alert.AlertLevel;
                existing.Geocode = alert.Geocode;
                existing.SyncedAt = alert.SyncedAt;
            }

            return Task.CompletedTask;
        }
    }
}
