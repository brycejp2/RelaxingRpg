using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RelaxingRpg.Engine;

/// <summary>
/// A 2D camera that produces a view matrix for <see cref="SpriteBatch.Begin"/>.
/// Keeps <see cref="Position"/> centered on screen. Matrix-based (no external
/// dependency); MonoGame.Extended's camera can replace this later if needed.
/// </summary>
public sealed class Camera
{
    private readonly GraphicsDevice _graphicsDevice;

    /// <summary>World-space point the camera is centered on.</summary>
    public Vector2 Position;

    public float Zoom = 2f;

    public Camera(GraphicsDevice graphicsDevice) => _graphicsDevice = graphicsDevice;

    public void Follow(Vector2 worldTarget) => Position = worldTarget;

    public Matrix GetViewMatrix()
    {
        var viewport = _graphicsDevice.Viewport;
        return Matrix.CreateTranslation(-Position.X, -Position.Y, 0f)
             * Matrix.CreateScale(Zoom, Zoom, 1f)
             * Matrix.CreateTranslation(viewport.Width / 2f, viewport.Height / 2f, 0f);
    }
}
