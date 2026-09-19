namespace Opus127.Dengue.Api.Integrations.AlertaDengue;

public interface IAlertaDengueClient
{
    Task<IReadOnlyList<AlertaDengueWeekDto>> GetCityAlertsAsync(
        int ewStart, int ewEnd, int eyStart, int eyEnd, CancellationToken cancellationToken);
}
