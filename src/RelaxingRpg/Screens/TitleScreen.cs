using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RelaxingRpg.Engine;

namespace RelaxingRpg.Screens;

/// <summary>
/// Placeholder title screen. Draws a gently pulsing banner (no fonts wired up yet —
/// that comes with the content pipeline in a later phase) and starts the game on
/// Enter/Space.
/// </summary>
public sealed class TitleScreen : GameScreen
{
    private Texture2D _pixel = null!;
    private double _time;

    public TitleScreen(RelaxingGame game) : base(game) { }

    public override void LoadContent()
    {
        _pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });
    }

    public override void Update(GameTime gameTime)
    {
        _time += gameTime.ElapsedGameTime.TotalSeconds;
        if (Game.Input.WasPressed(Keys.Enter) || Game.Input.WasPressed(Keys.Space))
            Game.Screens.Replace(new PlayScreen(Game));
    }

    public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        float pulse = 0.5f + 0.5f * (float)System.Math.Sin(_time * 2.0);
        var viewport = Game.GraphicsDevice.Viewport;
        var banner = new Rectangle(viewport.Width / 2 - 220, viewport.Height / 2 - 50, 440, 100);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        spriteBatch.Draw(_pixel, banner, Color.Lerp(Color.SaddleBrown, Color.Goldenrod, pulse));
        spriteBatch.End();
    }

    public override void Dispose() => _pixel.Dispose();
}
