namespace Opus127.Dengue.Api.Epidemiology;

public static class EpidemiologicalCalendar
{
    private static readonly TimeZoneInfo SaoPaulo =
        TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public static EpidemiologicalWeek FromDate(DateOnly date)
    {
        var week1ThisYear = StartOfWeekSunday(new DateOnly(date.Year, 1, 4));
        var week1NextYear = StartOfWeekSunday(new DateOnly(date.Year + 1, 1, 4));
        if (date < week1ThisYear)
            return FromDate(new DateOnly(date.Year - 1, 12, 31));
        if (date >= week1NextYear)
            return new EpidemiologicalWeek(date.Year + 1, 1);

        var sunday = StartOfWeekSunday(date);
        var week = ((sunday.DayNumber - week1ThisYear.DayNumber) / 7) + 1;
        return new EpidemiologicalWeek(date.Year, week);
    }

    public static DateOnly StartOfWeekSunday(DateOnly date)
    {
        var offset = (int)date.DayOfWeek; // Sunday = 0
        return date.AddDays(-offset);
    }

    public static IReadOnlyList<(int EyStart, int EwStart, int EyEnd, int EwEnd)> RangesForLastMonths(
        DateTimeOffset now, int months = 6)
    {
        var end = DateOnly.FromDateTime(now.UtcDateTime);
        var start = end.AddMonths(-months);
        var startWeek = FromDate(start);
        var endWeek = FromDate(end);
        if (startWeek.Year == endWeek.Year)
            return [(startWeek.Year, startWeek.Week, endWeek.Year, endWeek.Week)];

        return
        [
            (startWeek.Year, startWeek.Week, startWeek.Year, 53),
            (endWeek.Year, 1, endWeek.Year, endWeek.Week)
        ];
    }

    public static IReadOnlyList<EpidemiologicalWeek> LastWeeks(DateTimeOffset now, int count = 3)
    {
        var list = new List<EpidemiologicalWeek>(count);
        var today = DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(now, SaoPaulo).DateTime);
        var cursorSunday = StartOfWeekSunday(today).AddDays(-7);
        for (var i = 0; i < count; i++)
        {
            list.Add(FromDate(cursorSunday));
            cursorSunday = cursorSunday.AddDays(-7);
        }
        return list;
    }
}
