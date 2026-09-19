using FluentAssertions;
using Opus127.Dengue.Api.Epidemiology;

namespace Opus127.Dengue.Api.Tests;

public class EpidemiologicalCalendarTests
{
    [Fact]
    public void FromSe_splits_year_and_week_and_formats_contract_string()
    {
        var week = EpidemiologicalWeek.FromSe(202602);

        week.Year.Should().Be(2026);
        week.Week.Should().Be(2);
        week.ToContractString().Should().Be("2026-02");
        week.ToSe().Should().Be(202602);
    }

    [Fact]
    public void FromDate_on_january_4_2026_is_epidemiological_week_1()
    {
        var week = EpidemiologicalCalendar.FromDate(new DateOnly(2026, 1, 4));

        week.Year.Should().Be(2026);
        week.Week.Should().Be(1);
        week.ToContractString().Should().Be("2026-01");
    }

    [Fact]
    public void RangesForLastMonths_on_january_15_2026_spans_previous_year()
    {
        var now = new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero);

        var ranges = EpidemiologicalCalendar.RangesForLastMonths(now);

        ranges.Should().HaveCount(2);
        ranges[0].EyStart.Should().Be(2025);
        ranges[0].EyEnd.Should().Be(2025);
        ranges[0].EwEnd.Should().Be(53);
        ranges[1].EyStart.Should().Be(2026);
        ranges[1].EwStart.Should().Be(1);
        ranges[1].EyEnd.Should().Be(2026);
    }
}
