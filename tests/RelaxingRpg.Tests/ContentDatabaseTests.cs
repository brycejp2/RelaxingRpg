using RelaxingRpg.Core.Data;
using Xunit;

namespace RelaxingRpg.Tests;

public sealed class ContentDatabaseTests : IDisposable
{
    private readonly string _root;

    public ContentDatabaseTests()
    {
        _root = Path.Combine(Path.GetTempPath(), "relaxingrpg_tests_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(_root, "monsters"));
        Directory.CreateDirectory(Path.Combine(_root, "animals"));
        Directory.CreateDirectory(Path.Combine(_root, "items"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
            Directory.Delete(_root, recursive: true);
    }

    private void WriteDef(string folder, string fileName, string json) =>
        File.WriteAllText(Path.Combine(_root, folder, fileName), json);

    [Fact]
    public void LoadFromDirectory_IndexesDefsById()
    {
        WriteDef("monsters", "slime.json", """
            { "id": "green_slime", "displayName": "Green Slime", "maxHealth": 24,
              "ai": "wander_and_chase",
              "drops": [ { "item": "slime_goo", "chance": 0.75, "min": 1, "max": 2 } ] }
            """);

        var db = new ContentDatabase();
        db.LoadFromDirectory(_root);

        Assert.True(db.Monsters.ContainsKey("green_slime"));
        var slime = db.Monsters["green_slime"];
        Assert.Equal(24, slime.MaxHealth);
        Assert.Equal("wander_and_chase", slime.Ai);
        Assert.Single(slime.Drops);
        Assert.Equal("slime_goo", slime.Drops[0].Item);
    }

    [Fact]
    public void AddingNewCreature_RequiresNoCodeChange_JustAFile()
    {
        // The priority feature: a brand-new monster authored purely as data is
        // picked up by the database with no code edits.
        var db = new ContentDatabase();
        db.LoadFromDirectory(_root);
        Assert.False(db.Monsters.ContainsKey("fire_bat"));

        WriteDef("monsters", "fire_bat.json", """
            { "id": "fire_bat", "displayName": "Fire Bat", "maxHealth": 12,
              "moveSpeed": 70, "damage": 5, "ai": "fly_and_chase" }
            """);

        var reloaded = new ContentDatabase();
        reloaded.LoadFromDirectory(_root);

        Assert.True(reloaded.Monsters.ContainsKey("fire_bat"));
        Assert.Equal(5, reloaded.Monsters["fire_bat"].Damage);
    }

    [Fact]
    public void DuplicateId_Throws()
    {
        WriteDef("items", "a.json", """{ "id": "wood", "displayName": "Wood" }""");
        WriteDef("items", "b.json", """{ "id": "wood", "displayName": "Also Wood" }""");

        var db = new ContentDatabase();
        var ex = Assert.Throws<InvalidDataException>(() => db.LoadFromDirectory(_root));
        Assert.Contains("Duplicate content id", ex.Message);
    }

    [Fact]
    public void MissingId_Throws()
    {
        WriteDef("items", "bad.json", """{ "displayName": "No Id Here" }""");

        var db = new ContentDatabase();
        var ex = Assert.Throws<InvalidDataException>(() => db.LoadFromDirectory(_root));
        Assert.Contains("missing a required 'id'", ex.Message);
    }

    [Fact]
    public void MalformedJson_Throws()
    {
        WriteDef("items", "broken.json", "{ this is not json ");

        var db = new ContentDatabase();
        Assert.Throws<InvalidDataException>(() => db.LoadFromDirectory(_root));
    }

    [Fact]
    public void MissingFolders_AreSkipped()
    {
        var empty = Path.Combine(Path.GetTempPath(), "relaxingrpg_empty_" + Guid.NewGuid().ToString("N"));
        var db = new ContentDatabase();

        db.LoadFromDirectory(empty); // no throw

        Assert.Equal(0, db.TotalCount);
    }
}
