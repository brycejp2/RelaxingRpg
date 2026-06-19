using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RelaxingRpg.Core.Data;
using RelaxingRpg.Core.Time;
using RelaxingRpg.Engine;
using RelaxingRpg.Screens;

namespace RelaxingRpg;

/// <summary>
/// Root MonoGame game. Owns the shared services (content database, clock, input,
/// screen stack) and runs the standard update/draw loop, delegating per-frame work
/// to the active <see cref="GameScreen"/>.
/// </summary>
public sealed class RelaxingGame : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch = null!;

    public ScreenManager Screens { get; } = new();
    public ContentDatabase Data { get; } = new();
    public GameClock Clock { get; } = new();
    public InputManager Input { get; } = new();

    public RelaxingGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Window.Title = "RelaxingRpg";
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        var dataRoot = Path.Combine(AppContext.BaseDirectory, "Content", "data");
        Data.LoadFromDirectory(dataRoot);
        Console.WriteLine(
            $"[Content] Loaded {Data.Items.Count} items, {Data.Crops.Count} crops, " +
            $"{Data.Animals.Count} animals, {Data.Monsters.Count} monsters " +
            $"({Data.TotalCount} total) from {dataRoot}");

        Screens.Push(new TitleScreen(this));
    }

    protected override void Update(GameTime gameTime)
    {
        Input.Update();
        if (Input.IsExitRequested)
        {
            Exit();
            return;
        }

        Screens.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(40, 60, 50));
        Screens.Draw(gameTime, _spriteBatch);
        base.Draw(gameTime);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Screens.Dispose();
            _spriteBatch?.Dispose();
        }
        base.Dispose(disposing);
    }
}
