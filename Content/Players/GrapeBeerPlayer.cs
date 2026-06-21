using OldCalamityDrinks.Content.Buffs;
using OldCalamityDrinks.Content.Items;
using Terraria;
using Terraria.ModLoader;

namespace OldCalamityDrinks.Content.Players
{
    public class GrapeBeerPlayer : ModPlayer
    {
        public bool grapeBeer = false;
        public float critDamage = 0f;

        public override void ResetEffects()
        {
            grapeBeer = false;
            critDamage = 0f;
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += critDamage;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage += critDamage;
        }
    }
}
