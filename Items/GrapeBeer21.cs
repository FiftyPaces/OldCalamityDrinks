using CalamityMod;
using GrapeBeer21Mod.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GrapeBeer21Mod.Items
{
    public class GrapeBeer21 : ModItem
    {
        public static float CritLoss = 75;

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 20;
            ItemID.Sets.DrinkParticleColors[Type] = new Color[3] {
                new Color(36, 2, 41),
                new Color(56, 0, 64),
                new Color(82, 10, 92)
            };
        }

        public override void SetDefaults()
        {
            Item.DefaultToFood(12, 28, ModContent.BuffType<GrapeBeer21Buff>(), CalamityUtils.MinutesToFrames(6), true);

            Item.value = Item.sellPrice(silver: 2);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10).
                AddIngredient(ItemID.Bottle, 10).
                AddIngredient(ItemID.Grapes).
                AddTile(TileID.Kegs).
                Register();
        }
    }
}
