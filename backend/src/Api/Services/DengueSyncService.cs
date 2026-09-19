using Opus127.Dengue.Api.Entities;
using Opus127.Dengue.Api.Epidemiology;
using Opus127.Dengue.Api.Integrations.AlertaDengue;
using Opus127.Dengue.Api.Repositories;

namespace Opus127.Dengue.Api.Services;

public sealed class DengueSyncService : IDengueSyncService
{
    private readonly IAlertaDengueClient _client;
    private readonly IDengueAlertRepository _repository;
    private readonly string _geocode;

    public DengueSyncService(
        IAlertaDengueClient client,
        IDengueAlertRepository repository,
        IConfiguration configuration)
    {
        _client = client;
        _repository = repository;
        _geocode = configuration["AlertaDengue:Geocode"]!;
    }

    public async Task<int> SyncLastSixMonthsAsync(CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var ranges = EpidemiologicalCalendar.RangesForLastMonths(now, 6);
        var alerts = new List<DengueWeeklyAlert>();

        foreach (var (eyStart, ewStart, eyEnd, ewEnd) in ranges)
        {
            var weeks = await _client.GetCityAlertsAsync(ewStart, ewEnd, eyStart, eyEnd, cancellationToken);
            foreach (var dto in weeks)
            {
                var week = EpidemiologicalWeek.FromSe(dto.SE);
                alerts.Add(new DengueWeeklyAlert
                {
                    EpidemiologicalYear = week.Year,
                    EpidemiologicalWeek = week.Week,
                    EstimatedCases = dto.casos_est,
                    NotifiedCases = dto.casos,
                    AlertLevel = dto.nivel,
                    Geocode = _geocode,
                    SyncedAt = now
                });
            }
        }

        await _repository.UpsertRangeAsync(alerts, cancellationToken);
        return alerts.Count;
    }
}
