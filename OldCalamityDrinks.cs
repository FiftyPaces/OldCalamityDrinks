using System.Collections.Generic;
using OldCalamityDrinks.Content.Buffs;
using OldCalamityDrinks.Content.Items;
using Terraria.ModLoader;

namespace OldCalamityDrinks
{
    public class OldCalamityDrinks : Mod
    {
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("ImproveGame", out Mod improveGame))
            {
                try
                {
                    improveGame.Call("AddPotion",
                        ModContent.ItemType<GrapeBeer>(),
                        new List<int> { ModContent.BuffType<GrapeBeerDebuff>() });
                }
                catch { }
            }
        }
    }
}
