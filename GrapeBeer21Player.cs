using Terraria.ModLoader;

namespace GrapeBeer21Mod
{
    public class GrapeBeer21Player : ModPlayer
    {
        public bool grapeBeer21;

        public override void ResetEffects()
        {
            grapeBeer21 = false;
        }
    }
}
