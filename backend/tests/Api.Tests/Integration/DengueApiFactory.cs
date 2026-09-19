using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using Opus127.Dengue.Api.Data;
using Opus127.Dengue.Api.Entities;
using Opus127.Dengue.Api.Integrations.AlertaDengue;
using Testcontainers.MsSql;

namespace Opus127.Dengue.Api.Tests.Integration;

public sealed class DengueApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    public async Task InitializeAsync()
    {
        await _sqlServer.StartAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await _sqlServer.DisposeAsync();
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Dengue"] = _sqlServer.GetConnectionString()
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IAlertaDengueClient>();
            var stub = Substitute.For<IAlertaDengueClient>();
            stub.GetCityAlertsAsync(
                    Arg.Any<int>(),
                    Arg.Any<int>(),
                    Arg.Any<int>(),
                    Arg.Any<int>(),
                    Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<IReadOnlyList<AlertaDengueWeekDto>>([]));
            services.AddSingleton(stub);
        });
    }

    public async Task SeedKnownWeekAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DengueDbContext>();
        var exists = await db.DengueWeeklyAlerts.AnyAsync(alert =>
            alert.EpidemiologicalYear == 2023 && alert.EpidemiologicalWeek == 40);
        if (exists)
            return;

        db.DengueWeeklyAlerts.Add(new DengueWeeklyAlert
        {
            EpidemiologicalYear = 2023,
            EpidemiologicalWeek = 40,
            EstimatedCases = 45,
            NotifiedCases = 38,
            AlertLevel = 2,
            Geocode = "3106200",
            SyncedAt = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync();
    }
}
