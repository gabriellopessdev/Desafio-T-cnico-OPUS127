using System.Text.Json.Serialization;

namespace Opus127.Dengue.Api.Contracts;

public sealed record DengueWeekResponse(
    [property: JsonPropertyName("semana_epidemiologica")] string SemanaEpidemiologica,
    [property: JsonPropertyName("casos_est")] decimal CasosEst,
    [property: JsonPropertyName("casos_notificados")] int CasosNotificados,
    [property: JsonPropertyName("nivel_alerta")] int NivelAlerta);
