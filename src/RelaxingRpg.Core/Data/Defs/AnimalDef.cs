namespace RelaxingRpg.Core.Data.Defs;

/// <summary>What a farm animal produces, and how often.</summary>
public sealed class ProduceDef
{
    public string Item { get; set; } = "";
    public int EveryDays { get; set; } = 1;
    public bool RequiresFed { get; set; } = true;
}

/// <summary>
/// A tameable farm animal (chicken, cow, ...). Fully data-driven: adding a new
/// animal is just a new JSON file plus a sprite — no code changes required.
/// </summary>
public sealed class AnimalDef : ContentDef
{
    public string? Sprite { get; set; }

    /// <summary>Named animation clips mapping to sprite-sheet frame indices.</summary>
    public Dictionary<string, int[]>? Animations { get; set; }

    /// <summary>Optional produce schedule (eggs, milk, ...).</summary>
    public ProduceDef? Produces { get; set; }

    public int HappinessFromPetting { get; set; } = 5;

    /// <summary>Wander speed in pixels per second.</summary>
    public float MoveSpeed { get; set; } = 30f;
}
