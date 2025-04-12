using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WaterGuns.Modules.Weapons;
using WaterGuns.Players;
using WaterGuns.Utils;
using WaterGuns.Sources;

namespace WaterGuns.Weapons.Shotgun;

public class Shotgun : BaseWeapon
{
    public override string Texture => TexturesPath.Weapons + "Shotgun/Shotgun";

    public SoundModule Sound { get; private set; }
    public PropertyModule Property { get; private set; }
    public PumpModule Pump { get; private set; }
    public SpriteModule Sprite { get; private set; }

    public Shotgun() : base()
    {
        Sound = new SoundModule();
        Property = new PropertyModule();
        Pump = new PumpModule();
        Sprite = new SpriteModule();

        Composite.AddModule(Sound, Property, Pump);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

        Sound.SetWater(this);
        Property.SetProjectile<ShotProjectile>(this);

        Property.SetProperties(this, 78, 26, 14, 3f, 16f, 38, 38, 22f, ItemRarityID.Blue, Item.buyPrice(0, 5, 25, 0));
        Sprite.SefDefaults(new Vector2(56f, 56f), new Vector2(-12, 0));

        Pump.SetDefaults(10);
    }

    public override void HoldItem(Terraria.Player player)
    {
        base.HoldItem(player);

        Pump.Update();

        DoAltUse(player);
    }

    public override void AltUseAlways(Player player)
    {
        if (Pump.Pumped && player.GetModPlayer<ShotPlayer>().Chain == null)
        {
            var direction = Main.MouseWorld - player.Center;
            direction.Normalize();
            direction *= 24f;

            Helper.SpawnProjectile<ChainProjectile>(Item.GetSource_FromThis(), player, player.Center, direction, Item.damage, Item.knockBack);

            Pump.Reset();
        }
    }

    public override bool Shoot(Terraria.Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source,
                               Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 velocity,
                               int type, int damage, float knockback)
    {
        base.Shoot(player, source, position, velocity, type, damage, knockback);

        var spreads = new int[] { -2, -1, 1, 2 };
        var shotPlayer = player.GetModPlayer<ShotPlayer>();
        var custom = new WeaponWithAmmoSource(source, this);

        foreach (var spread in spreads)
        {
            var positionCopy = Sprite.ApplyOffset(position, velocity);
            var velocityCopy = Property.ApplyInaccuracy(velocity);

            var up = velocity.RotatedBy(-MathHelper.PiOver2);
            up.Normalize();
            positionCopy += up * spread * Main.rand.NextFloat(2f, 3f);
            velocityCopy *= Main.rand.NextFloat(0.6f, 1f);

            if (shotPlayer.IsPulling)
            {
                var start = velocity.RotatedBy(-MathHelper.PiOver4);
                var end = velocity.RotatedBy(MathHelper.PiOver4);
                Particle.Arc(DustID.Smoke, positionCopy, new Vector2(8, 8), start, end, 3, 2f, 1.4f, 0, 55);

                Helper.SpawnProjectile<WaterBalloon>(custom, player, positionCopy, velocityCopy, damage, knockback);
            }
            else
            {
                Helper.SpawnProjectile<ShotProjectile>(custom, player, positionCopy, velocityCopy, damage, knockback);
            }
        }

        if (shotPlayer.IsPulling)
        {
            shotPlayer.IsPulling = false;

            SoundEngine.PlaySound(SoundID.Item36);
            player.GetModPlayer<ScreenShake>().Activate(6, 4);

            var direction = player.Center - Main.MouseWorld;
            direction.Normalize();
            var dist = (player.Center - shotPlayer.Target!.Center).Length();
            dist = (1 / dist) * Main.ViewSize.X;
            dist = MathHelper.Clamp(dist, 3f, 10f);
            direction *= dist;
            player.velocity = direction;

            shotPlayer.Chain!.Projectile.Kill();
        }

        return false;
    }

    public override void ModifyTooltips(List<TooltipLine> tooltip)
    {
        base.ModifyTooltips(tooltip);

        Sprite.AddAmmoTooltip(tooltip, Mod);
    }

    public override Vector2? HoldoutOffset()
    {
        return Sprite.HoldoutOffset;
    }
}
