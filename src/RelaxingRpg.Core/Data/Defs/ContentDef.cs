namespace RelaxingRpg.Core.Data.Defs;

/// <summary>
/// Base class for every data-driven content definition loaded from JSON.
/// Each definition is keyed by its <see cref="Id"/>. Game code only ever refers
/// to content by string id, never by a hardcoded type, so new content can be
/// added by dropping in a JSON file plus a sprite.
/// </summary>
public abstract class ContentDef
{
    /// <summary>Unique, stable identifier (e.g. "green_slime"). Required.</summary>
    public string Id { get; set; } = "";

    /// <summary>Human-readable name shown in UI.</summary>
    public string DisplayName { get; set; } = "";
}
