using System;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GrapeBeer21Mod.Buffs
{
    public class GrapeBeer21Buff : ModBuff
    {
        private static Type _infPlayerType;
        private static PropertyInfo _blacklistProp;
        private static MethodInfo _containsMethod;
        private static Mod _improveGame;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;

            // 缓存 ImproveGame 黑名单反射信息
            if (ModLoader.TryGetMod("ImproveGame", out _improveGame))
            {
                _infPlayerType = _improveGame.Code.GetType("ImproveGame.Modules.InfiniteBuff.InfiniteBuffModPlayer");
                _blacklistProp = _infPlayerType?.GetProperty("Blacklist");
                _containsMethod = _blacklistProp?.PropertyType.GetMethod("ContainsByType");
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // 检查 ImproveGame 黑名单，若被禁用则跳过效果
            if (IsBlacklisted(player))
                return;

            player.GetModPlayer<GrapeBeer21Player>().grapeBeer21 = true;
        }

        private bool IsBlacklisted(Player player)
        {
            if (_infPlayerType == null || _blacklistProp == null || _containsMethod == null)
                return false;

            try
            {
                var infPlayer = typeof(Player)
                    .GetMethod("GetModPlayer", System.Type.EmptyTypes)
                    ?.MakeGenericMethod(_infPlayerType)
                    .Invoke(player, null);
                if (infPlayer == null) return false;

                var blacklist = _blacklistProp.GetValue(infPlayer);
                if (blacklist == null) return false;

                return (bool)_containsMethod.Invoke(blacklist, new object[] { Type });
            }
            catch
            {
                return false;
            }
        }
    }
}
