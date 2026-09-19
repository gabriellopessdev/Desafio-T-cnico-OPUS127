using Opus127.Dengue.Api.Entities;

namespace Opus127.Dengue.Api.Repositories;

public interface IDengueAlertRepository
{
    Task<DengueWeeklyAlert?> GetAsync(int year, int week, CancellationToken cancellationToken);

    Task UpsertRangeAsync(IEnumerable<DengueWeeklyAlert> alerts, CancellationToken cancellationToken);
}
