using System;
using System.Reflection;
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
        public bool buffActive = false;

        /// <summary>
        /// Cached once per frame in PostUpdate (after BE's PostUpdateBuffs cleanup).
        /// True if BE has blacklisted GrapeBeer.
        /// </summary>
        public bool disabledByBE = false;

        // Cached effect state from PostUpdate, applied next frame in ResetEffects
        // for defense/moveSpeed (which must run before movement processing).
        private bool _effectActive;

        // Cached reflection info for BE's InfiniteBuffModPlayer.Blacklist
        private static Type _beModPlayerType;
        private static PropertyInfo _beBlacklistProperty;
        private static MethodInfo _beContainsByTypeMethod;
        private static MethodInfo _playerGetModPlayer;
        private static bool _beInitAttempted;

        public override void ResetEffects()
        {
            // Apply defense and move speed from last frame's PostUpdate result.
            // PostUpdate runs after BE's cleanup, so _effectActive is authoritative.
            // (1-frame delay at 60fps is imperceptible.)
            if (_effectActive)
            {
                int baseDefense = Player.statDefense;
                Player.statDefense -= (int)(baseDefense * 0.03f);
                Player.moveSpeed *= 0.95f;
            }

            grapeBeer = false;
            critDamage = 0f;
            buffActive = false;
            disabledByBE = false;
        }

        /// <summary>
        /// PostUpdate runs AFTER PostUpdateBuffs, so BE's CleanupBuffByBlacklist
        /// has already executed. The authoritative effect state is computed here.
        /// </summary>
        public override void PostUpdate()
        {
            BuffTypeCache();

            // Check if the buff is currently active on the player
            bool hasBuff = false;
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Player.buffType[i] == _cachedBuffType && Player.buffTime[i] > 0)
                {
                    hasBuff = true;
                    break;
                }
            }

            // If BetterExperience has blacklisted this buff, treat as inactive
            if (hasBuff && IsBlacklistedByBE())
            {
                disabledByBE = true;
                grapeBeer = false;
                critDamage = 0f;
                _effectActive = false;
                return;
            }

            if (hasBuff)
            {
                grapeBeer = true;
                critDamage -= GrapeBeer.CritLoss * 0.01f;
                _effectActive = true;
            }
            else
            {
                _effectActive = false;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (grapeBeer && !disabledByBE)
            {
                modifiers.CritDamage += critDamage;
            }
        }

        private static int _cachedBuffType = -1;
        private static void BuffTypeCache()
        {
            if (_cachedBuffType < 0)
                _cachedBuffType = ModContent.BuffType<GrapeBeerDebuff>();
        }

        private bool IsBlacklistedByBE()
        {
            InitBEReflection();
            if (_beModPlayerType == null || _beBlacklistProperty == null || _beContainsByTypeMethod == null)
                return false;

            try
            {
                var getModPlayer = _playerGetModPlayer.MakeGenericMethod(_beModPlayerType);
                var infBuffPlayer = getModPlayer.Invoke(Player, null);
                if (infBuffPlayer == null) return false;

                var blacklist = _beBlacklistProperty.GetValue(infBuffPlayer);
                if (blacklist == null) return false;

                return (bool)_beContainsByTypeMethod.Invoke(blacklist, new object[] { _cachedBuffType });
            }
            catch
            {
                return false;
            }
        }

        private static void InitBEReflection()
        {
            if (_beInitAttempted) return;
            _beInitAttempted = true;

            if (!ModLoader.TryGetMod("ImproveGame", out Mod improveGame)) return;

            try
            {
                _beModPlayerType = improveGame.Code.GetType(
                    "ImproveGame.Modules.InfiniteBuff.InfiniteBuffModPlayer");
                if (_beModPlayerType == null) return;

                _beBlacklistProperty = _beModPlayerType.GetProperty("Blacklist");
                if (_beBlacklistProperty == null) return;

                var blacklistType = _beBlacklistProperty.PropertyType;
                _beContainsByTypeMethod = blacklistType.GetMethod("ContainsByType");
                if (_beContainsByTypeMethod == null) return;

                _playerGetModPlayer = typeof(Player).GetMethod("GetModPlayer",
                    BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);
                if (_playerGetModPlayer == null) return;
            }
            catch { /* BE integration is optional */ }
        }
    }
}
