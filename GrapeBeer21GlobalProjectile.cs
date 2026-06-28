using CalamityMod;
using CalamityMod.Systems.Collections;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace GrapeBeer21Mod
{
    public class GrapeBeer21GlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool grapeBeer21;
        public float homingRange;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            void Apply()
            {
                grapeBeer21 = true;
                homingRange = 600;
                if (projectile.timeLeft > 300 * projectile.MaxUpdates)
                    projectile.timeLeft = 300 * projectile.MaxUpdates;
                projectile.usesLocalNPCImmunity = true;
                projectile.localNPCHitCooldown = -1;
            }

            if (source is EntitySource_ItemUse_WithAmmo { Item: Item item })
            {
                if (source is EntitySource_Parent { Entity: Player player })
                {
                    if (player.GetModPlayer<GrapeBeer21Player>().grapeBeer21
                        && (item.useAmmo == AmmoID.Bullet
                         || item.useAmmo == AmmoID.Arrow
                         || item.useAmmo == AmmoID.Dart
                         || item.useAmmo == AmmoID.Rocket))
                    {
                        if (player.heldProj != projectile.whoAmI
                            && projectile.aiStyle != ProjAIStyleID.HeldProjectile
                            && projectile.damage > 0
                            && !CalamityProjectileSets.DoesNotGetHomingWithGrapeBeer[projectile.type])
                            Apply();
                        else
                            grapeBeer21 = true;
                    }
                }
            }
            else if (source is EntitySource_Parent { Entity: Projectile parent })
            {
                if (parent.TryGetGlobalProjectile(out GrapeBeer21GlobalProjectile pg) && pg.grapeBeer21)
                {
                    // 子弹幕只继承追踪，不改免疫/生命值，避免爆炸弹无法二次伤害
                    grapeBeer21 = true;
                    if (Main.player[projectile.owner].heldProj != projectile.whoAmI
                        && projectile.aiStyle != ProjAIStyleID.HeldProjectile
                        && projectile.damage > 0
                        && !CalamityProjectileSets.DoesNotGetHomingWithGrapeBeer[projectile.type])
                        homingRange = 600;
                }
            }
        }

        public override void PostAI(Projectile projectile)
        {
            if (homingRange > 0f
                && Main.player[projectile.owner].heldProj != projectile.whoAmI
                && projectile.aiStyle != ProjAIStyleID.HeldProjectile)
            {
                CalamityUtils.HomeInOnNPC(projectile, !projectile.tileCollide, homingRange, 12f, 20f, true);
            }
        }
    }
}
