namespace RelaxingRpg.Core.Data.Defs;

/// <summary>A single entry in a monster's loot table.</summary>
public sealed class DropDef
{
    public string Item { get; set; } = "";

    /// <summary>Probability (0..1) that this drop occurs on death.</summary>
    public double Chance { get; set; } = 1.0;

    public int Min { get; set; } = 1;
    public int Max { get; set; } = 1;
}

/// <summary>
/// A hostile creature. Behaviour is selected by the <see cref="Ai"/> string, which
/// maps to a small registry of behaviour functions, so most new monsters reuse an
/// existing behaviour with different stats. Adding a new monster is just a new JSON
/// file plus a sprite.
/// </summary>
public sealed class MonsterDef : ContentDef
{
    public string? Sprite { get; set; }

    /// <summary>Named animation clips mapping to sprite-sheet frame indices.</summary>
    public Dictionary<string, int[]>? Animations { get; set; }

    public int MaxHealth { get; set; } = 10;

    /// <summary>Movement speed in pixels per second.</summary>
    public float MoveSpeed { get; set; } = 40f;

    public int Damage { get; set; } = 1;

    /// <summary>Behaviour key resolved against the AI behaviour registry (e.g. "wander_and_chase").</summary>
    public string Ai { get; set; } = "wander";

    public List<DropDef> Drops { get; set; } = new();
}
