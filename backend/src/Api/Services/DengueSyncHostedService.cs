namespace Opus127.Dengue.Api.Services;

public sealed class DengueSyncHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DengueSyncHostedService> _logger;

    public DengueSyncHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<DengueSyncHostedService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => SyncOnceAsync(stoppingToken);

    internal async Task SyncOnceAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var sync = scope.ServiceProvider.GetRequiredService<IDengueSyncService>();
            await sync.SyncLastSixMonthsAsync(stoppingToken);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Falha HTTP ao sincronizar alertas da AlertaDengue.");
        }
        catch (TaskCanceledException exception)
        {
            _logger.LogError(exception, "Sincronização da AlertaDengue cancelada ou expirou.");
        }
    }
}
