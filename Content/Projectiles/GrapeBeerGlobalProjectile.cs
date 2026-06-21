using Microsoft.Xna.Framework;
using OldCalamityDrinks.Content.Buffs;
using OldCalamityDrinks.Content.Players;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace OldCalamityDrinks.Content.Projectiles
{
    public class GrapeBeerGlobalProjectile : GlobalProjectile
    {
        public float conditionalHomingRange = 0f;
        public bool grapeBeer = false;
        public int defExtraUpdates = -1;

        public override bool InstancePerEntity => true;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // === Grape Beer ammo-based homing ===
            if (source is EntitySource_ItemUse_WithAmmo { Item: Item item })
            {
                if (source is EntitySource_Parent { Entity: Player player })
                {
                    GrapeBeerPlayer modPlayer = player.GetModPlayer<GrapeBeerPlayer>();
                    if (modPlayer.grapeBeer &&
                        (item.useAmmo == AmmoID.Bullet || item.useAmmo == AmmoID.Arrow ||
                         item.useAmmo == AmmoID.Dart || item.useAmmo == AmmoID.Rocket))
                    {
                        if (player.heldProj != projectile.whoAmI &&
                            projectile.aiStyle != ProjAIStyleID.HeldProjectile &&
                            projectile.damage > 0)
                        {
                            ApplyGrapeBeer(projectile);
                        }
                        else
                        {
                            grapeBeer = true;
                        }
                    }
                }
            }

            // === Grape Beer chain homing (child projectiles inherit from parent) ===
            if (source is EntitySource_Parent { Entity: NPC npc })
            {
            }
            else if (source is EntitySource_Parent { Entity: Projectile parent })
            {
                GrapeBeerGlobalProjectile parentGlobal = parent.GetGlobalProjectile<GrapeBeerGlobalProjectile>();
                if (parentGlobal.grapeBeer)
                {
                    if (Main.player[projectile.owner].heldProj != projectile.whoAmI &&
                        projectile.aiStyle != ProjAIStyleID.HeldProjectile &&
                        projectile.damage > 0)
                    {
                        ApplyGrapeBeer(projectile);
                    }
                    else
                    {
                        grapeBeer = true;
                    }
                }
            }
        }

        private static void ApplyGrapeBeer(Projectile projectile)
        {
            GrapeBeerGlobalProjectile g = projectile.GetGlobalProjectile<GrapeBeerGlobalProjectile>();
            g.grapeBeer = true;
            g.conditionalHomingRange = 600;
            if (projectile.timeLeft > 300 * projectile.MaxUpdates)
                projectile.timeLeft = 300 * projectile.MaxUpdates;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;
        }

        private static readonly int BuffType = ModContent.BuffType<GrapeBeerDebuff>();

        public override void PostAI(Projectile projectile)
        {
            GrapeBeerGlobalProjectile g = projectile.GetGlobalProjectile<GrapeBeerGlobalProjectile>();

            // grapeBeer and disabledByBE are computed in GrapeBeerPlayer.PostUpdate(),
            // which runs AFTER BetterExperience's PostUpdateBuffs (CleanupBuffByBlacklist).
            // So they always reflect the correct state for this frame.
            Player owner = Main.player[projectile.owner];
            GrapeBeerPlayer modPlayer = owner.GetModPlayer<GrapeBeerPlayer>();
            if (g.conditionalHomingRange > 0f &&
                modPlayer.grapeBeer &&
                !modPlayer.disabledByBE &&
                owner.heldProj != projectile.whoAmI &&
                projectile.aiStyle != ProjAIStyleID.HeldProjectile)
            {
                HomeInOnNPC(projectile, g, !projectile.tileCollide, g.conditionalHomingRange, 12f, 20f, true);
            }
        }

        /// <summary>
        /// Clone of CalamityUtils.HomeInOnNPC, matching Calamity 2.1.2 behavior exactly.
        /// </summary>
        private static void HomeInOnNPC(Projectile projectile, GrapeBeerGlobalProjectile g,
            bool ignoreTiles, float distanceRequired, float homingVelocity, float inertia, bool respectIFrames)
        {
            if (!projectile.friendly)
                return;

            if (g.defExtraUpdates == -1)
                g.defExtraUpdates = projectile.extraUpdates;

            Vector2 destination = projectile.Center;
            float maxDistance = distanceRequired;
            bool locatedTarget = false;

            float npcDistCompare = 25000f;
            int index = -1;
            foreach (NPC n in Main.ActiveNPCs)
            {
                float extraDistance = (n.width / 2) + (n.height / 2);
                if (!n.CanBeChasedBy(projectile, false) ||
                    !projectile.WithinRange(n.Center, maxDistance + extraDistance) ||
                    (respectIFrames && (projectile.localNPCImmunity[n.whoAmI] > 0 ||
                                        projectile.localNPCImmunity[n.whoAmI] == -1 ||
                                        n.immune[projectile.owner] > 0)))
                    continue;

                float currentNPCDist = Vector2.Distance(n.Center, projectile.Center);
                if (respectIFrames &&
                    Projectile.perIDStaticNPCImmunity[projectile.type][n.whoAmI] > Main.GameUpdateCount)
                    currentNPCDist += 1600;

                if ((currentNPCDist < npcDistCompare) &&
                    (ignoreTiles || Collision.CanHit(projectile.Center, 1, 1, n.Center, 1, 1)))
                {
                    npcDistCompare = currentNPCDist;
                    index = n.whoAmI;
                }
            }

            if (index != -1)
            {
                destination = Main.npc[index].Center;
                locatedTarget = true;
            }

            if (locatedTarget)
            {
                projectile.extraUpdates = g.defExtraUpdates + 1;

                Vector2 homeDirection = (destination - projectile.Center).SafeNormalize(Vector2.UnitY);
                projectile.velocity = (projectile.velocity * inertia + homeDirection * homingVelocity) / (inertia + 1f);
            }
            else
            {
                projectile.extraUpdates = g.defExtraUpdates;
            }
        }
    }
}
