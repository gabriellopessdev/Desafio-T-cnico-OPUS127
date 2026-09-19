using System.Text.Json.Serialization;

namespace Opus127.Dengue.Api.Contracts;

public sealed record SyncResponse(
    [property: JsonPropertyName("upserted")] int Upserted);
