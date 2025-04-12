using Microsoft.Xna.Framework;
using Terraria;
using WaterGuns.Modules.Projectiles;
using WaterGuns.Utils;
using WaterGuns.Sources;
using Terraria.DataStructures;

namespace WaterGuns.Weapons.Wooden;

public class WoodenProjectile : BaseProjectile
{
    public override string Texture => TexturesPath.Empty;

    public PropertyModule Property { get; private set; }
    public WaterModule Water { get; private set; }

    public WoodenProjectile() : base()
    {
        var immunity = new ImmunityModule();
        immunity.SetDefaults();
        Composite.AddRuntimeModule(immunity);

        Property = new PropertyModule();
        Water = new WaterModule();

        Composite.AddModule(Property, Water);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

        Water.SetDefaults();
        Property.SetProperties(this, 16, 16, 1, 1);
        Property.SetTimeLeft(this, 35);
        Property.SetGravity();
    }

    public override void OnSpawn(IEntitySource source)
    {
        base.OnSpawn(source);

        if (source is WeaponWithAmmoSource custom)
        {
            custom.Ammo?.ApplyToProjectile(this);
        }
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

        Water.KillEffect(Projectile.Center, Projectile.velocity);
    }

    public override void OnHitNPC(Terraria.NPC target, Terraria.NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);

        if (Main.rand.Next(0, 4) == 0)
        {
            var position = target.Center - new Vector2(0, target.height * 1.5f + Main.rand.NextFloat(0f, 6f));

            Helper.SpawnProjectile<AcornProjectile>(Projectile.GetSource_FromThis(), Owner, position, Vector2.Zero,
                                                    hit.Damage, hit.Knockback);
        }
    }

    public override void AI()
    {
        base.AI();

        Projectile.velocity = Property.ApplyGravity(Projectile.velocity);
        Water.CreateDust(Projectile.Center, Projectile.velocity);
    }
}
