namespace RelaxingRpg.Core.Data.Defs;

/// <summary>A stackable item: crops, produce, resources, tools, etc.</summary>
public sealed class ItemDef : ContentDef
{
    public string? Sprite { get; set; }

    /// <summary>Maximum stack size in a single inventory slot.</summary>
    public int StackSize { get; set; } = 99;

    /// <summary>Base shop sell price in gold.</summary>
    public int SellPrice { get; set; }

    /// <summary>Free-form category used for sorting/filtering (e.g. "seed", "crop", "resource").</summary>
    public string Category { get; set; } = "misc";
}
