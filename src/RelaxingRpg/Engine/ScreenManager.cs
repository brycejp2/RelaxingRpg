using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RelaxingRpg.Engine;

/// <summary>
/// A simple stack of <see cref="GameScreen"/>s. Only the top screen updates and
/// draws. Pushing layers a new screen on top; replacing swaps the top screen.
/// </summary>
public sealed class ScreenManager : IDisposable
{
    private readonly Stack<GameScreen> _screens = new();

    public GameScreen? Current => _screens.Count > 0 ? _screens.Peek() : null;

    public void Push(GameScreen screen)
    {
        screen.LoadContent();
        _screens.Push(screen);
    }

    public void Pop()
    {
        if (_screens.Count > 0)
            _screens.Pop().Dispose();
    }

    public void Replace(GameScreen screen)
    {
        Pop();
        Push(screen);
    }

    public void Update(GameTime gameTime) => Current?.Update(gameTime);

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch) => Current?.Draw(gameTime, spriteBatch);

    public void Dispose()
    {
        while (_screens.Count > 0)
            _screens.Pop().Dispose();
    }
}
