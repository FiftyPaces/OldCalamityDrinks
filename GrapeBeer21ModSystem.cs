using GrapeBeer21Mod.Buffs;
using GrapeBeer21Mod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace GrapeBeer21Mod
{
    public class GrapeBeer21ModSystem : ModSystem
    {
        public override void PostSetupContent()
        {
            // 通过 ImproveGame 的跨模组 API 注册，自动获得"更好的体验"UI开关功能
            if (ModLoader.TryGetMod("ImproveGame", out Mod improveGame))
            {
                improveGame.Call("AddPotion",
                    ModContent.ItemType<GrapeBeer21>(),
                    new List<int> { ModContent.BuffType<GrapeBeer21Buff>() }
                );
            }
        }
    }
}
