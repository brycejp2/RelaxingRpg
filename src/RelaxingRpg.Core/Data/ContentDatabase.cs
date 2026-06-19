using System.Text.Json;
using RelaxingRpg.Core.Data.Defs;

namespace RelaxingRpg.Core.Data;

/// <summary>
/// Loads and indexes every data-driven content definition from a directory tree of
/// JSON files. This is the backbone of the game's content: crops, items, animals,
/// and monsters are all authored as JSON and looked up here by string id.
///
/// Adding a brand-new monster or animal is just dropping a JSON file into the
/// matching folder — no recompile of game logic.
/// </summary>
public sealed class ContentDatabase
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private readonly Dictionary<string, ItemDef> _items = new(StringComparer.Ordinal);
    private readonly Dictionary<string, CropDef> _crops = new(StringComparer.Ordinal);
    private readonly Dictionary<string, AnimalDef> _animals = new(StringComparer.Ordinal);
    private readonly Dictionary<string, MonsterDef> _monsters = new(StringComparer.Ordinal);

    public IReadOnlyDictionary<string, ItemDef> Items => _items;
    public IReadOnlyDictionary<string, CropDef> Crops => _crops;
    public IReadOnlyDictionary<string, AnimalDef> Animals => _animals;
    public IReadOnlyDictionary<string, MonsterDef> Monsters => _monsters;

    public int TotalCount => _items.Count + _crops.Count + _animals.Count + _monsters.Count;

    /// <summary>
    /// Load all content from <paramref name="dataRoot"/>. Expects subfolders
    /// "items", "crops", "animals", "monsters". Missing folders are skipped so the
    /// game still starts during early development.
    /// </summary>
    public void LoadFromDirectory(string dataRoot)
    {
        LoadFolder(Path.Combine(dataRoot, "items"), _items);
        LoadFolder(Path.Combine(dataRoot, "crops"), _crops);
        LoadFolder(Path.Combine(dataRoot, "animals"), _animals);
        LoadFolder(Path.Combine(dataRoot, "monsters"), _monsters);
    }

    private static void LoadFolder<T>(string dir, Dictionary<string, T> target) where T : ContentDef
    {
        if (!Directory.Exists(dir))
            return;

        foreach (var file in Directory.EnumerateFiles(dir, "*.json", SearchOption.AllDirectories))
        {
            T def;
            try
            {
                var json = File.ReadAllText(file);
                def = JsonSerializer.Deserialize<T>(json, JsonOptions)
                      ?? throw new InvalidDataException($"Content file '{file}' deserialized to null.");
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"Failed to parse content file '{file}': {ex.Message}", ex);
            }

            if (string.IsNullOrWhiteSpace(def.Id))
                throw new InvalidDataException($"Content file '{file}' is missing a required 'id'.");

            if (!target.TryAdd(def.Id, def))
                throw new InvalidDataException($"Duplicate content id '{def.Id}' found in '{file}'.");
        }
    }
}
