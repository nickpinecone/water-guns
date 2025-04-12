using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace WaterGuns.Modules.Projectiles;

public class ChainModule : IModule
{
    public Asset<Texture2D>? Texture { get; private set; }
    public Rectangle Source { get; private set; }

    public void SetTexture(string path, Rectangle rect)
    {
        Texture = ModContent.Request<Texture2D>(path);
        Source = rect;
    }

    public void DrawChain(Vector2 from, Vector2 to)
    {
        var source = Source;
        var segmentLength = source.Height;

        var origin = source.Size() / 2f;
        var direction = to - from;
        var unitDirection = direction.SafeNormalize(Vector2.Zero);

        var rotation = unitDirection.ToRotation() + MathHelper.PiOver2;
        var drawLength = direction.Length() + segmentLength / 2f;

        while (drawLength > 0f)
        {
            var color = Lighting.GetColor(from.ToTileCoordinates());

            Main.spriteBatch.Draw(Texture!.Value, from - Main.screenPosition, source, color, rotation, origin, 1f,
                                  SpriteEffects.None, 0f);

            from += unitDirection * segmentLength;
            drawLength -= segmentLength;
        }
    }
}
