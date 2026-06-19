# Plan: "RelaxingRpg" — a Stardew Valley-style farming RPG in MonoGame

## Context

The repository is empty (no commits, no tracked files). The goal is to build a
relaxing, Stardew Valley–style 2D farming RPG in C#/.NET. Based on the user's
choices:

- **Framework:** MonoGame — the same XNA-derived framework Stardew Valley itself
  is built on. Code-first, 2D-focused, total control, maximum learning value.
- **Deliverable:** A **prototype-first roadmap** — get a playable vertical slice
  quickly, then layer systems on top.
- **Special priority:** *Easy authoring of new monsters/animals.* This pushes the
  whole architecture to be **data-driven**: creatures, crops, items, and tiles are
  described in JSON content files + a sprite, so adding new content needs **no code
  changes**. This is the single most important architectural decision in the plan
  and it shapes Phase 0 onward.

The intended outcome: a working game executable that grows from "walk around a
farm" to "till/plant/grow/harvest, day cycle, save/load, animals, and monsters,"
with a content pipeline that makes new creatures trivial to add.

## Tech stack & key dependencies

- **.NET 8** + **MonoGame 3.8** (DesktopGL template — cross-platform: Windows/Linux/Mac).
- **MonoGame.Extended** — provides Tiled map loading, camera2D, sprite/animation
  helpers, ECS, and input listeners. Saves writing a lot of boilerplate. We will
  lean on its `OrthographicCamera`, Tiled importer, and animated sprites.
- **Tiled** (free external map editor, `.tmx`/`.tsx`) — author the farm/town maps
  visually instead of hardcoding tile arrays. Loaded via MonoGame.Extended.
- **System.Text.Json** — load data-driven content definitions (crops, animals,
  monsters, items) from `Content/data/*.json`.
- (Optional later) **Newtonsoft.Json** only if we need polymorphic converters that
  are awkward in System.Text.Json.

## Proposed project / solution structure

```
RelaxingRpg.sln
src/
  RelaxingRpg/                 # main MonoGame DesktopGL project
    Game1.cs                   # entry, root game loop, screen manager wiring
    Core/
      ScreenManager.cs         # stack of game screens (Title, Play, Menu)
      Camera.cs                # wraps MonoGame.Extended OrthographicCamera
      Time/GameClock.cs        # in-game minutes/hours/days/seasons
      Input/InputManager.cs    # keyboard/mouse -> action mapping
    World/
      TileMap.cs               # loaded Tiled map + collision + tile metadata
      Tile.cs                  # tile state (tilled, watered, occupant)
      Farm.cs                  # farm-specific tile state grid (crops planted)
    Entities/                  # ECS-lite or MonoGame.Extended ECS
      Player.cs
      Creature.cs              # shared base for animals + monsters (data-driven)
      Components/              # Transform, Sprite, Animation, Movement, Health, AI
    Farming/
      Crop.cs                  # runtime crop instance (growth stage, watered)
      CropSystem.cs            # daily growth tick, harvest logic
    Items/
      Item.cs, Inventory.cs, ItemDatabase.cs
    Data/                      # *** data-driven content loading ***
      ContentDatabase.cs       # loads + indexes all JSON definitions
      Defs/CropDef.cs, AnimalDef.cs, MonsterDef.cs, ItemDef.cs, TileDef.cs
    Save/
      SaveGame.cs, SaveSystem.cs   # JSON serialize world+player+farm state
    Screens/
      TitleScreen.cs, PlayScreen.cs, InventoryScreen.cs
  Content/
    Content.mgcb               # MonoGame content pipeline (textures, fonts, maps)
    data/                      # JSON content (NO rebuild needed to edit)
      crops/*.json
      animals/*.json
      monsters/*.json
      items/*.json
    sprites/...  maps/...  fonts/...
tests/
  RelaxingRpg.Tests/          # xUnit — unit-test systems that have no rendering
```

## The data-driven content system (the priority feature)

This is what makes "create new monsters or animals" easy. Each creature/crop/item
is a JSON file loaded at startup into `ContentDatabase`. Code references things by
string **id**, never by hardcoded type.

Example `Content/data/monsters/slime.json`:
```json
{
  "id": "green_slime",
  "displayName": "Green Slime",
  "kind": "monster",
  "sprite": "sprites/creatures/slime",
  "animations": { "idle": [0,1], "move": [2,3,4,5] },
  "maxHealth": 24,
  "moveSpeed": 40,
  "damage": 3,
  "ai": "wander_and_chase",
  "drops": [ { "item": "slime_goo", "chance": 0.75, "min": 1, "max": 2 } ]
}
```

Example `Content/data/animals/chicken.json`:
```json
{
  "id": "chicken",
  "displayName": "Chicken",
  "kind": "animal",
  "sprite": "sprites/creatures/chicken",
  "produces": { "item": "egg", "everyDays": 1, "requiresFed": true },
  "happinessFromPetting": 8
}
```

- `CreatureFactory.Spawn("green_slime")` builds a `Creature` entity from the def —
  sprite, stats, AI behavior, and drops all come from data.
- **AI is keyed by a string** (`"wander_and_chase"`) mapped to a small registry of
  behavior functions, so new monsters reuse existing behaviors with new stats; only
  genuinely new behaviors need code.
- **Adding a new animal or monster = add one JSON file + a sprite. No recompile of
  game logic.** This is verified explicitly in the roadmap (Phase 5).

## Phased roadmap (prototype-first)

**Phase 0 — Scaffold & content foundation**
- Create solution, MonoGame DesktopGL project, add MonoGame.Extended + tests.
- Stand up `ScreenManager`, `GameClock`, `InputManager`, `Camera`.
- Build `ContentDatabase` + def classes and JSON loading *first* (so everything
  later is data-driven from day one).
- Deliverable: blank window, content DB loads sample JSON, logs counts.

**Phase 1 — World & movement (vertical slice base)**
- Load a Tiled farm map; render layers; tile collision.
- Player entity: 4-direction movement + walk animation, camera follows.
- Deliverable: walk around a tiled farm with collisions.

**Phase 2 — Core farming loop**
- Tool/action system: till soil, water, plant seed.
- `CropSystem` daily growth using `CropDef` (stages, days-per-stage, season).
- Harvest crop → item into inventory.
- Deliverable: the Stardew core loop — till, plant, water, grow, harvest.

**Phase 3 — Time, days & save/load**
- Day/night cycle from `GameClock`; sleep advances day and ticks crop growth.
- `SaveSystem` serializes player, farm tiles, crops, time to JSON; load on launch.
- Deliverable: play across multiple days; quit and resume.

**Phase 4 — Inventory & items UI**
- Hotbar + inventory screen, item stacking, equip/select tool.
- `ItemDatabase` from `items/*.json`.
- Deliverable: usable inventory and tool selection.

**Phase 5 — Creatures: animals & monsters (the priority pillar)**
- `CreatureFactory` spawns from `AnimalDef`/`MonsterDef`.
- Animals: roam farm/coop, can be fed/petted, produce items on a schedule.
- Monsters: spawn in a cave/dungeon map area, basic AI (wander/chase), take damage
  from a tool/weapon, drop items via `drops` table.
- **Acceptance test for the priority feature:** add a brand-new monster and a
  brand-new animal *purely by adding JSON + a sprite*, confirm they appear and
  behave with zero code edits.
- Deliverable: animals on the farm, monsters in the cave, both data-authored.

**Phase 6+ (later, not part of the first slice)**
- NPCs/relationships, shops/economy, fishing, foraging, festivals, seasons art,
  crafting, sound/music. Each follows the same data-driven pattern.

## Critical files to be created first (Phases 0–1)

- `RelaxingRpg.sln`, `src/RelaxingRpg/RelaxingRpg.csproj` (DesktopGL, .NET 8)
- `src/RelaxingRpg/Game1.cs`, `Core/ScreenManager.cs`, `Core/Camera.cs`
- `src/RelaxingRpg/Data/ContentDatabase.cs` + `Data/Defs/*.cs` (data foundation)
- `src/RelaxingRpg/World/TileMap.cs`, `Entities/Player.cs`
- `Content/Content.mgcb`, `Content/data/**`, a sample Tiled map + sprites
- `tests/RelaxingRpg.Tests/` with first tests for `ContentDatabase` + `GameClock`

## Verification

- **Build:** `dotnet build RelaxingRpg.sln` succeeds on .NET 8.
- **Run:** `dotnet run --project src/RelaxingRpg` launches a window.
- **Unit tests:** `dotnet test` — cover non-rendering logic: JSON content loading
  (`ContentDatabase` indexes defs by id), `GameClock` advancement, `CropSystem`
  growth ticks, drop-table rolls, save/load round-trip (serialize → deserialize →
  equal state).
- **Manual per-phase smoke tests:** the "Deliverable" line of each phase is its
  acceptance check (walk around, complete the farm loop, save & resume, etc.).
- **Priority-feature gate (Phase 5):** add `Content/data/monsters/<new>.json` and
  `Content/data/animals/<new>.json` plus sprites, launch, and confirm the new
  creatures spawn and behave **without editing any `.cs` file**. This proves the
  data-driven authoring goal.

## Open considerations (decide as we build, not blockers)

- Art: use free placeholder tilesets/sprites (e.g. Kenney, open CC0 packs) for the
  prototype; swap later. No art is on the critical path.
- ECS depth: start "ECS-lite" (plain component classes on entities); adopt
  MonoGame.Extended's full ECS only if entity count/perf demands it.
