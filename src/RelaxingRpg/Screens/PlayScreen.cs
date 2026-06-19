using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RelaxingRpg.Engine;

namespace RelaxingRpg.Screens;

/// <summary>
/// The in-world screen. For this scaffold it renders a checkerboard ground and a
/// player square you can walk around with WASD/arrows; the camera follows and the
/// in-game clock ticks (shown in the window title). Real tiles, tools and crops
/// land in the next phases.
/// </summary>
public sealed class PlayScreen : GameScreen
{
    private const int TileSize = 16;
    private const float PlayerSpeed = 90f;            // pixels per second
    private const double RealSecondsPerGameMinute = 0.7; // ~7s real = 10 in-game minutes

    private Texture2D _pixel = null!;
    private Camera _camera = null!;
    private Vector2 _player = Vector2.Zero;
    private double _minuteAccumulator;

    public PlayScreen(RelaxingGame game) : base(game) { }

    public override void LoadContent()
    {
        _pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
        _camera = new Camera(Game.GraphicsDevice);
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _player += Game.Input.MovementAxis * PlayerSpeed * dt;
        _camera.Follow(_player + new Vector2(TileSize / 2f, TileSize / 2f));

        _minuteAccumulator += gameTime.ElapsedGameTime.TotalSeconds;
        while (_minuteAccumulator >= RealSecondsPerGameMinute)
        {
            _minuteAccumulator -= RealSecondsPerGameMinute;
            Game.Clock.AdvanceMinutes(1);
        }

        Game.Window.Title = $"RelaxingRpg — {Game.Clock}";
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

        for (int gx = -12; gx <= 12; gx++)
        for (int gy = -12; gy <= 12; gy++)
        {
            var color = ((gx + gy) & 1) == 0 ? new Color(70, 100, 70) : new Color(82, 112, 82);
            spriteBatch.Draw(_pixel, new Rectangle(gx * TileSize, gy * TileSize, TileSize, TileSize), color);
        }

        spriteBatch.Draw(
            _pixel,
            new Rectangle((int)_player.X, (int)_player.Y, TileSize, TileSize),
            Color.Goldenrod);

        spriteBatch.End();
    }

    public override void Dispose() => _pixel.Dispose();
}
