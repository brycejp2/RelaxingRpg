namespace RelaxingRpg.Core.Data.Defs;

/// <summary>
/// A plantable crop. Growth is described purely by data: each entry in
/// <see cref="GrowthStageDays"/> is the number of in-game days that growth stage
/// lasts before advancing to the next.
/// </summary>
public sealed class CropDef : ContentDef
{
    public string? Sprite { get; set; }

    /// <summary>Season the crop can grow in ("spring", "summer", "fall", "winter").</summary>
    public string Season { get; set; } = "spring";

    /// <summary>Days spent in each growth stage; the sum is the total days to maturity.</summary>
    public int[] GrowthStageDays { get; set; } = Array.Empty<int>();

    /// <summary>Item id granted on harvest.</summary>
    public string HarvestItem { get; set; } = "";

    /// <summary>How many of <see cref="HarvestItem"/> a single harvest yields.</summary>
    public int HarvestYield { get; set; } = 1;

    /// <summary>If true, the crop regrows after harvest instead of being consumed.</summary>
    public bool Regrows { get; set; }

    /// <summary>Days to regrow when <see cref="Regrows"/> is true.</summary>
    public int RegrowDays { get; set; }

    /// <summary>Seed item id used to plant this crop.</summary>
    public string SeedItem { get; set; } = "";

    /// <summary>Total days from planting to first harvest.</summary>
    public int DaysToMaturity
    {
        get
        {
            int total = 0;
            foreach (var d in GrowthStageDays) total += d;
            return total;
        }
    }
}
