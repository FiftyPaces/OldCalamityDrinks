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
        public bool disabledByBE = false;

        public override void ResetEffects()
        {
            grapeBeer = false;
            critDamage = 0f;
            disabledByBE = false;
        }

        public override void PostUpdate()
        {
            int buffType = ModContent.BuffType<GrapeBeerDebuff>();
            if (Player.HasBuff(buffType) && IsBlacklistedByBE(buffType))
                disabledByBE = true;
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (grapeBeer && !disabledByBE)
                modifiers.CritDamage += critDamage;
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (grapeBeer && !disabledByBE)
                modifiers.CritDamage += critDamage;
        }

        #region BE Blacklist
        private static Type _beType;
        private static PropertyInfo _beBlacklist;
        private static MethodInfo _beContains;
        private static MethodInfo _getModPlayer;

        private bool IsBlacklistedByBE(int buffType)
        {
            InitBE();
            if (_beType == null) return false;
            try
            {
                var p = _getModPlayer.MakeGenericMethod(_beType).Invoke(Player, null);
                var bl = _beBlacklist.GetValue(p);
                return (bool)_beContains.Invoke(bl, new object[] { buffType });
            }
            catch { return false; }
        }

        private static void InitBE()
        {
            if (_beType != null) return;
            if (!ModLoader.TryGetMod("ImproveGame", out Mod m)) return;
            try
            {
                _beType = m.Code.GetType("ImproveGame.Modules.InfiniteBuff.InfiniteBuffModPlayer");
                if (_beType == null) return;
                _beBlacklist = _beType.GetProperty("Blacklist");
                _beContains = _beBlacklist.PropertyType.GetMethod("ContainsByType");
                _getModPlayer = typeof(Player).GetMethod("GetModPlayer",
                    BindingFlags.Public | BindingFlags.Instance, Type.EmptyTypes);
            }
            catch { }
        }
        #endregion
    }
}
