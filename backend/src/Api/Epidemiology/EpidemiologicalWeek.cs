namespace Opus127.Dengue.Api.Epidemiology;

public readonly record struct EpidemiologicalWeek(int Year, int Week)
{
    public string ToContractString() => $"{Year}-{Week:D2}";

    public static EpidemiologicalWeek FromSe(int se)
    {
        var year = se / 100;
        var week = se % 100;
        return new EpidemiologicalWeek(year, week);
    }

    public int ToSe() => Year * 100 + Week;
}
