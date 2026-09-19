namespace Opus127.Dengue.Api.Services;

public interface IDengueSyncService
{
    Task<int> SyncLastSixMonthsAsync(CancellationToken cancellationToken);
}
