using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using WaterGuns.Modules.Projectiles;
using WaterGuns.Utils;

namespace WaterGuns.Weapons.Golden;

public class DaggerProjectile : BaseProjectile
{
    public override string Texture => TexturesPath.Weapons + "Golden/DaggerProjectile";

    public Animation<int> Appear { get; private set; }
    public PropertyModule Property { get; private set; }
    public StickModule Stick { get; private set; }

    public SoundStyle SlashSound { get; private set; }
    public Vector2 InitialVelocity { get; set; } = Vector2.Zero;

    private Vector2 _beforeHit = Vector2.Zero;
    private int _penetrateAmount = 0;
    private int _penetrateMax = 3;

    public DaggerProjectile() : base()
    {
        Composite.AddRuntimeModule(ImmunityModule.New(20));

        Property = new PropertyModule();
        Stick = new StickModule();

        Composite.AddModule(Property, Stick);

        Appear = new Animation<int>(10);

        SlashSound = new SoundStyle(AudioPath.Impact + "Slash")
        {
            Volume = 0.5f,
            PitchVariance = 0.1f,
        };
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

        Property.SetProperties(this, 10, 24, 1, -1, 0, 255, 0, false);
        Property.SetTimeLeft(this, 60);
    }

    public override bool? CanHitNPC(NPC target)
    {
        if (Stick.Target == null && !target.friendly && target.getRect().Intersects(Projectile.getRect()))
        {
            SoundEngine.PlaySound(SlashSound);

            _beforeHit = Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            Stick.ToTarget(target, Projectile.Center);
        }

        return base.CanHitNPC(target);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);

        if (_penetrateAmount < _penetrateMax)
        {
            Projectile.timeLeft = Property.DefaultTime;
            _penetrateAmount += 1;

            var invert = _beforeHit.RotatedBy(MathHelper.Pi);
            var start = invert.RotatedBy(-MathHelper.PiOver4);
            var end = invert.RotatedBy(MathHelper.PiOver4);

            var offset = _beforeHit.SafeNormalize(Vector2.Zero);
            Particle.Arc(DustID.Blood, (Vector2)Stick.HitPoint! + offset * (Projectile.height / 2), new Vector2(6, 6),
                         start, end, 3, 2f, 1f);
        }
        else
        {
            Projectile.timeLeft = 10;
        }
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

        if (_penetrateAmount < _penetrateMax)
        {
            var particle = Particle.Single(DustID.Iron, Projectile.Center, new Vector2(6, 6), Vector2.Zero, 1f);
            particle.noGravity = true;
        }
    }

    public override void AI()
    {
        base.AI();

        if (_penetrateAmount >= _penetrateMax)
        {
            Projectile.friendly = false;
            Projectile.alpha = Appear.Backwards() ?? Projectile.alpha;
        }
        else
        {
            Projectile.alpha = Appear.Animate(255, 0) ?? Projectile.alpha;
        }

        if (Appear.Finished)
        {
            if (Stick.Target == null)
            {
                Projectile.velocity = InitialVelocity;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            else if (Stick.Target != null)
            {
                if (Stick.Update() == null)
                {
                    Projectile.Kill();
                }
                else
                {
                    Projectile.Center = (Vector2)Stick.HitPoint!;
                }
            }
        }
    }
}
