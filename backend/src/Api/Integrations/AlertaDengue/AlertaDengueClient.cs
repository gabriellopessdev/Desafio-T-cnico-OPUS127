using System.Net.Http.Json;
using System.Text.Json;

namespace Opus127.Dengue.Api.Integrations.AlertaDengue;

public sealed class AlertaDengueClient : IAlertaDengueClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly string _geocode;

    public AlertaDengueClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _geocode = configuration["AlertaDengue:Geocode"]!;
    }

    public async Task<IReadOnlyList<AlertaDengueWeekDto>> GetCityAlertsAsync(
        int ewStart, int ewEnd, int eyStart, int eyEnd, CancellationToken cancellationToken)
    {
        var path =
            $"api/alertcity?geocode={Uri.EscapeDataString(_geocode)}&disease=dengue&format=json&ew_start={ewStart}&ew_end={ewEnd}&ey_start={eyStart}&ey_end={eyEnd}";

        var weeks = await _httpClient.GetFromJsonAsync<List<AlertaDengueWeekDto>>(
            path, JsonOptions, cancellationToken);

        return weeks ?? [];
    }
}
