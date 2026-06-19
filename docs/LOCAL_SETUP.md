# Continuing RelaxingRpg locally

This project was started in a Claude Code **web** session running in an ephemeral
cloud container. The chat transcript of that session stays in the cloud and can't
be moved to your machine — but all the work is committed and pushed, so you can
pick it right back up locally. This guide gets you there.

## 1. Install prerequisites

- **.NET 8 SDK** — https://dotnet.microsoft.com/download/dotnet/8.0
  (verify with `dotnet --version` → should print `8.x`)
- **Git**
- **Claude Code CLI** (optional, to keep working with Claude):
  `npm install -g @anthropic-ai/claude-code` — see https://code.claude.com/docs

## 2. Get the code

> Note: clone from GitHub. The `origin` remote inside the cloud container points
> at an internal proxy (`http://127.0.0.1:...`) that only works there.

```bash
git clone https://github.com/brycejp2/relaxingrpg.git
cd relaxingrpg
git checkout claude/blissful-mayer-wqptap
```

## 3. Build, test, and run

```bash
dotnet build RelaxingRpg.sln          # compile all three projects
dotnet test                           # run the unit tests (should be all green)
dotnet run --project src/RelaxingRpg  # launch the game window
```

In the game: **Enter/Space** at the title screen, walk with **WASD / arrow keys**,
**Esc** to quit. The in-game clock ticks and shows in the window title.

Running the window needs a real display, which is why it can only be launched
locally (not in the headless cloud container).

## 4. Keep working with Claude Code

From the repo root:

```bash
claude
```

Then orient it, for example:

> Continue the RelaxingRpg game. Phase 0 (scaffold + data-driven content) is done
> and committed on branch `claude/blissful-mayer-wqptap`. Read `docs/PLAN.md` and
> start Phase 1 (Tiled tile world + animated player + tile collision).

It will read [`docs/PLAN.md`](PLAN.md) and the codebase and continue from the same
roadmap.

## Optional: MonoGame content-pipeline editor

You don't need it to build — the game references the `MonoGame.Framework.DesktopGL`
NuGet package directly. But once you start adding sprites/fonts/maps through a
`.mgcb` content file (Phase 1+), install the editor locally:

```bash
dotnet tool install --global dotnet-mgcb-editor
mgcb-editor                            # launches the visual content pipeline tool
```

(This tool needs a desktop environment, so it only runs on a real machine.)
