using Microsoft.EntityFrameworkCore;
using Opus127.Dengue.Api.Data;
using Opus127.Dengue.Api.Entities;

namespace Opus127.Dengue.Api.Repositories;

public sealed class DengueAlertRepository : IDengueAlertRepository
{
    private readonly DengueDbContext _db;

    public DengueAlertRepository(DengueDbContext db)
    {
        _db = db;
    }

    public Task<DengueWeeklyAlert?> GetAsync(int year, int week, CancellationToken cancellationToken)
    {
        return _db.DengueWeeklyAlerts
            .FirstOrDefaultAsync(
                alert => alert.EpidemiologicalYear == year && alert.EpidemiologicalWeek == week,
                cancellationToken);
    }

    public async Task UpsertRangeAsync(IEnumerable<DengueWeeklyAlert> alerts, CancellationToken cancellationToken)
    {
        foreach (var alert in alerts)
        {
            var existing = await GetAsync(alert.EpidemiologicalYear, alert.EpidemiologicalWeek, cancellationToken);
            if (existing is null)
            {
                _db.DengueWeeklyAlerts.Add(alert);
                continue;
            }

            existing.EstimatedCases = alert.EstimatedCases;
            existing.NotifiedCases = alert.NotifiedCases;
            existing.AlertLevel = alert.AlertLevel;
            existing.Geocode = alert.Geocode;
            existing.SyncedAt = alert.SyncedAt;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }
}
