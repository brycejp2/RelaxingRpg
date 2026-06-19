# RelaxingRpg

A Stardew Valley–style 2D farming RPG built in C# / .NET 8 on
[MonoGame](https://www.monogame.net/) — the same framework Stardew Valley itself
uses.

The design goal that shapes the architecture: **adding new content (especially
monsters and animals) should be as easy as dropping in a JSON file plus a sprite,
with no code changes.** Everything is data-driven.

## Project layout

```
src/
  RelaxingRpg.Core/     Pure .NET domain logic (no MonoGame) — clock, content DB,
                        content definitions. Fast to unit-test.
  RelaxingRpg/          The MonoGame DesktopGL game (window, input, rendering,
                        screens). Content/data/**.json holds all game content.
tests/
  RelaxingRpg.Tests/    xUnit tests for the Core logic.
```

## Data-driven content

Game content lives in `src/RelaxingRpg/Content/data/` as JSON, grouped by type:
`items/`, `crops/`, `animals/`, `monsters/`. At startup `ContentDatabase` loads and
indexes every file by its `id`. To add a new monster:

```jsonc
// src/RelaxingRpg/Content/data/monsters/fire_bat.json
{
  "id": "fire_bat",
  "displayName": "Fire Bat",
  "sprite": "sprites/creatures/fire_bat",
  "maxHealth": 12,
  "moveSpeed": 70,
  "damage": 5,
  "ai": "fly_and_chase",
  "drops": [ { "item": "bat_wing", "chance": 0.5, "min": 1, "max": 1 } ]
}
```

That's it — no recompiling game logic.

## Build & run

```bash
dotnet build RelaxingRpg.sln          # compile everything
dotnet test                           # run the unit tests
dotnet run --project src/RelaxingRpg  # launch the game (needs a display)
```

In the game: press **Enter/Space** at the title, walk with **WASD / arrow keys**,
**Esc** to quit. The in-game clock ticks and is shown in the window title.

## Continuing locally

Cloning fresh or moving from a Claude Code web session to your own machine? See
[`docs/LOCAL_SETUP.md`](docs/LOCAL_SETUP.md) for prerequisites, the branch to check
out, and how to resume development with Claude Code locally.

## Roadmap

See [`docs/PLAN.md`](docs/PLAN.md) for the full phased plan. Current status:
**Phase 0 — scaffold + data-driven content foundation** is in place.
