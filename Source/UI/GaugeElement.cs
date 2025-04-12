using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Terraria;

namespace WaterGuns.UI;

public class GaugeElement : UIImage
{
    public static Asset<Texture2D>? Texture = null;

    public int Current { get; set; } = 0;
    public int Max { get; set; } = 0;
    public string Tooltip { get; set; } = "";
    public bool Hidden { get; set; } = false;
    public bool Active { get; set; } = true;

    public Color ColorA { get; set; } = Color.Blue;
    public Color ColorB { get; set; } = Color.Cyan;
    public Color ColorBorder { get; set; } = Color.White;
    public Color ColorBorderFull { get; set; } = Color.Gold;
    public Color ColorBorderInactive { get; set; } = Color.Gray;

    public GaugeElement() : base(Texture)
    {
        Width.Set(18, 0);
        Height.Set(90, 0);
    }

    public GaugeElement(Asset<Texture2D> texture) : this()
    {
    }

    public GaugeElement(Texture2D nonReloadingTexture) : this()
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Hidden)
        {
            return;
        }

        base.Draw(spriteBatch);
        this.DisplayTooltip();

        float percent = Current / (float)Max;

        if (!Active)
        {
            Color = ColorBorderInactive;
        }
        else if (percent >= 1f)
        {
            Color = ColorBorderFull;
        }
        else
        {
            Color = ColorBorder;
        }

        Rectangle rectangle = GetInnerDimensions().ToRectangle();
        rectangle.X += 2;
        rectangle.Width -= 4;
        rectangle.Y += 2;
        rectangle.Height -= 4;

        int steps = (int)((rectangle.Bottom - rectangle.Top) * percent);

        for (int i = 0; i < steps; i += 1)
        {
            float gradient = (float)i / (rectangle.Bottom - rectangle.Top);

            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                             new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - i, rectangle.Width, 1),
                             Color.Lerp(ColorA, ColorB, gradient));
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (Hidden || !Active)
        {
            return;
        }

        base.Update(gameTime);
    }

    private void DisplayTooltip()
    {
        if (IsMouseHovering)
        {
            Main.hoverItemName = Tooltip;
        }
    }
}
