using Opus127.Dengue.Api.Contracts;
using Opus127.Dengue.Api.Epidemiology;
using Opus127.Dengue.Api.Repositories;

namespace Opus127.Dengue.Api.Services;

public sealed class DengueQueryService : IDengueQueryService
{
    private readonly IDengueAlertRepository _repository;

    public DengueQueryService(IDengueAlertRepository repository)
    {
        _repository = repository;
    }

    public async Task<DengueWeekResponse?> GetByWeekAsync(int ew, int ey, CancellationToken cancellationToken)
    {
        if (ew is < 1 or > 53)
            throw new ArgumentOutOfRangeException(nameof(ew), ew, "A semana epidemiológica deve estar entre 1 e 53.");
        if (ey is < 2000 or > 2100)
            throw new ArgumentOutOfRangeException(nameof(ey), ey, "O ano epidemiológico deve estar entre 2000 e 2100.");

        var alert = await _repository.GetAsync(ey, ew, cancellationToken);
        if (alert is null)
            return null;

        var week = new EpidemiologicalWeek(alert.EpidemiologicalYear, alert.EpidemiologicalWeek);
        return new DengueWeekResponse(
            week.ToContractString(),
            alert.EstimatedCases,
            alert.NotifiedCases,
            alert.AlertLevel);
    }
}
