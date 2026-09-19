namespace Opus127.Dengue.Api.Entities;

public sealed class DengueWeeklyAlert
{
    public int Id { get; set; }
    public int EpidemiologicalYear { get; set; }
    public int EpidemiologicalWeek { get; set; }
    public decimal EstimatedCases { get; set; }
    public int NotifiedCases { get; set; }
    public int AlertLevel { get; set; }
    public string Geocode { get; set; } = "3106200";
    public DateTimeOffset SyncedAt { get; set; }
}
