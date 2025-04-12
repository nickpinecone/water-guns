using WaterGuns.Modules.Projectiles;
using WaterGuns.Utils;

namespace WaterGuns.Weapons.Sea;

public class BubbleExplosion : BaseProjectile
{
    public override string Texture => TexturesPath.Empty;

    public PropertyModule Property { get; private set; }

    public BubbleExplosion()
    {
        var immunity = new ImmunityModule();
        immunity.SetDefaults();
        Composite.AddRuntimeModule(immunity);

        Property = new PropertyModule();

        Composite.AddModule(Property);
    }

    public override void SetDefaults()
    {
        base.SetDefaults();

        Property.SetProperties(this, 20, 20, 1, 1, 0, 0, 100, false);
        Property.SetTimeLeft(this, 15);
    }
}
