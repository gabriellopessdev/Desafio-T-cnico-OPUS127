using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace Opus127.Dengue.Api.Tests.Integration;

[Collection(DengueApiCollection.Name)]
public sealed class DengueGetTests
{
    private readonly HttpClient _client;
    private readonly DengueApiFactory _factory;

    public DengueGetTests(DengueApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact(Timeout = 180_000)]
    public async Task Get_known_week_returns_contract_json()
    {
        await _factory.SeedKnownWeekAsync();

        var response = await _client.GetAsync("/api/dengue?ew=40&ey=2023");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("semana_epidemiologica").GetString().Should().Be("2023-40");
        json.GetProperty("casos_est").GetDecimal().Should().Be(45);
        json.GetProperty("casos_notificados").GetInt32().Should().Be(38);
        json.GetProperty("nivel_alerta").GetInt32().Should().Be(2);
    }

    [Fact(Timeout = 180_000)]
    public async Task Get_invalid_year_returns_bad_request()
    {
        var response = await _client.GetAsync("/api/dengue?ew=1&ey=1999");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact(Timeout = 180_000)]
    public async Task Get_missing_week_returns_not_found()
    {
        var response = await _client.GetAsync("/api/dengue?ew=1&ey=2020");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

[CollectionDefinition(DengueApiCollection.Name)]
public sealed class DengueApiCollection : ICollectionFixture<DengueApiFactory>
{
    public const string Name = "dengue-api";
}
