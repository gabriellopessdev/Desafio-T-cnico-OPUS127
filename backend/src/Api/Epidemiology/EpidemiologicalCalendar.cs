namespace Opus127.Dengue.Api.Epidemiology;

public static class EpidemiologicalCalendar
{
    public static EpidemiologicalWeek FromDate(DateOnly date)
    {
        var jan4 = new DateOnly(date.Year, 1, 4);
        var week1Sunday = StartOfWeekSunday(jan4);
        if (date < week1Sunday)
            return FromDate(new DateOnly(date.Year - 1, 12, 31));

        var sunday = StartOfWeekSunday(date);
        var week = ((sunday.DayNumber - week1Sunday.DayNumber) / 7) + 1;
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
        var cursorSunday = StartOfWeekSunday(DateOnly.FromDateTime(now.UtcDateTime));
        for (var i = 0; i < count; i++)
        {
            list.Add(FromDate(cursorSunday));
            cursorSunday = cursorSunday.AddDays(-7);
        }
        return list;
    }
}
