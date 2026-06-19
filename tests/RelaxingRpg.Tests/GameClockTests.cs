using RelaxingRpg.Core.Time;
using Xunit;

namespace RelaxingRpg.Tests;

public sealed class GameClockTests
{
    [Fact]
    public void NewClock_StartsAtSixAmOnDayOne()
    {
        var clock = new GameClock();

        Assert.Equal(6, clock.Hour);
        Assert.Equal(0, clock.Minute);
        Assert.Equal(1, clock.DayOfSeason);
        Assert.Equal(Season.Spring, clock.Season);
        Assert.Equal(1, clock.Year);
    }

    [Fact]
    public void AdvanceMinutes_WithinDay_DoesNotChangeDay()
    {
        var clock = new GameClock();

        int daysCrossed = clock.AdvanceMinutes(90); // 6:00 -> 7:30

        Assert.Equal(0, daysCrossed);
        Assert.Equal(7, clock.Hour);
        Assert.Equal(30, clock.Minute);
        Assert.Equal(0, clock.TotalDays);
    }

    [Fact]
    public void AdvanceMinutes_PastMidnight_RollsToNextDay()
    {
        var clock = new GameClock();

        int daysCrossed = clock.AdvanceMinutes(GameClock.MinutesPerDay); // a full 24h

        Assert.Equal(1, daysCrossed);
        Assert.Equal(1, clock.TotalDays);
        Assert.Equal(6, clock.Hour); // back to the same time of day
        Assert.Equal(2, clock.DayOfSeason);
    }

    [Fact]
    public void Season_And_Year_Advance_AfterEnoughDays()
    {
        var clock = new GameClock();

        // 28 days per season * 4 seasons = one full year.
        for (int i = 0; i < GameClock.DaysPerSeason * GameClock.SeasonsPerYear; i++)
            clock.SleepUntilMorning();

        Assert.Equal(Season.Spring, clock.Season);
        Assert.Equal(2, clock.Year);
        Assert.Equal(1, clock.DayOfSeason);
    }

    [Fact]
    public void AdvanceMinutes_Negative_Throws()
    {
        var clock = new GameClock();
        Assert.Throws<ArgumentOutOfRangeException>(() => clock.AdvanceMinutes(-1));
    }
}
