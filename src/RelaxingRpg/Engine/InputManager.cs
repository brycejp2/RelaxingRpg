using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RelaxingRpg.Engine;

/// <summary>
/// Thin wrapper over keyboard state that exposes edge-triggered presses and a
/// normalized movement axis (WASD / arrow keys). Call <see cref="Update"/> once per
/// frame before reading anything.
/// </summary>
public sealed class InputManager
{
    private KeyboardState _current;
    private KeyboardState _previous;

    public void Update()
    {
        _previous = _current;
        _current = Keyboard.GetState();
    }

    public bool IsExitRequested => _current.IsKeyDown(Keys.Escape);

    public bool IsKeyDown(Keys key) => _current.IsKeyDown(key);

    /// <summary>True only on the frame the key transitions from up to down.</summary>
    public bool WasPressed(Keys key) => _current.IsKeyDown(key) && _previous.IsKeyUp(key);

    /// <summary>Normalized movement direction from WASD / arrow keys (zero if idle).</summary>
    public Vector2 MovementAxis
    {
        get
        {
            var axis = Vector2.Zero;
            if (IsKeyDown(Keys.W) || IsKeyDown(Keys.Up)) axis.Y -= 1f;
            if (IsKeyDown(Keys.S) || IsKeyDown(Keys.Down)) axis.Y += 1f;
            if (IsKeyDown(Keys.A) || IsKeyDown(Keys.Left)) axis.X -= 1f;
            if (IsKeyDown(Keys.D) || IsKeyDown(Keys.Right)) axis.X += 1f;
            return axis == Vector2.Zero ? axis : Vector2.Normalize(axis);
        }
    }
}
