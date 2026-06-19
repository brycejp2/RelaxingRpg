using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RelaxingRpg.Engine;

/// <summary>
/// Base class for a single screen of the game (title, play, inventory, ...).
/// Screens manage their own <see cref="SpriteBatch"/> begin/end so each can use a
/// different transform (e.g. a world camera vs. screen-space UI).
/// </summary>
public abstract class GameScreen : IDisposable
{
    protected RelaxingGame Game { get; }

    protected GameScreen(RelaxingGame game) => Game = game;

    /// <summary>Called once when the screen is pushed onto the stack.</summary>
    public virtual void LoadContent() { }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }

    public virtual void Dispose() { }
}
