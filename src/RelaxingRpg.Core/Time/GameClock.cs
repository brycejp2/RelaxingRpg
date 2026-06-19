namespace RelaxingRpg.Core.Time;

/// <summary>The four in-game seasons, in calendar order.</summary>
public enum Season
{
    Spring,
    Summer,
    Fall,
    Winter,
}

/// <summary>
/// Pure-logic in-game clock and calendar. Tracks elapsed days and the minute of
/// the current day, and derives hour/minute/season/year. Has no dependency on
/// MonoGame so it can be unit-tested directly.
/// </summary>
public sealed class GameClock
{
    public const int MinutesPerHour = 60;
    public const int HoursPerDay = 24;
    public const int MinutesPerDay = MinutesPerHour * HoursPerDay; // 1440
    public const int DaysPerSeason = 28;
    public const int SeasonsPerYear = 4;

    /// <summary>The in-game day starts at 6:00 AM (matching the Stardew convention).</summary>
    public const int DayStartMinute = 6 * MinutesPerHour;

    /// <summary>Days elapsed since the start of the save (day 0 == the very first day).</summary>
    public int TotalDays { get; private set; }

    /// <summary>Minutes since midnight of the current day (0..1439).</summary>
    public int MinuteOfDay { get; private set; }

    public GameClock()
    {
        MinuteOfDay = DayStartMinute;
    }

    public int Hour => MinuteOfDay / MinutesPerHour;
    public int Minute => MinuteOfDay % MinutesPerHour;

    /// <summary>Day within the current season, 1..28.</summary>
    public int DayOfSeason => (TotalDays % DaysPerSeason) + 1;

    public Season Season => (Season)((TotalDays / DaysPerSeason) % SeasonsPerYear);

    /// <summary>Year, starting at 1.</summary>
    public int Year => (TotalDays / (DaysPerSeason * SeasonsPerYear)) + 1;

    /// <summary>
    /// Advance the clock by a number of in-game minutes. Rolls over into new days
    /// as needed and returns how many day boundaries were crossed.
    /// </summary>
    public int AdvanceMinutes(int minutes)
    {
        if (minutes < 0)
            throw new ArgumentOutOfRangeException(nameof(minutes), "Cannot advance time by a negative amount.");

        int daysAdvanced = 0;
        MinuteOfDay += minutes;
        while (MinuteOfDay >= MinutesPerDay)
        {
            MinuteOfDay -= MinutesPerDay;
            TotalDays++;
            daysAdvanced++;
        }
        return daysAdvanced;
    }

    /// <summary>Sleep: skip to 6:00 AM of the next day.</summary>
    public void SleepUntilMorning()
    {
        TotalDays++;
        MinuteOfDay = DayStartMinute;
    }

    public override string ToString() => $"Y{Year} {Season} {DayOfSeason}, {Hour:D2}:{Minute:D2}";
}
