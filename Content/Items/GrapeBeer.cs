using OldCalamityDrinks.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OldCalamityDrinks.Content.Items
{
    public class GrapeBeer : ModItem
    {
        public static float CritLoss = 75;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTime = 17;
            Item.useAnimation = 17;
            Item.consumable = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(silver: 2);
            Item.buffType = ModContent.BuffType<GrapeBeerDebuff>();
            Item.buffTime = 21600; // 6 minutes
            Item.width = 12;
            Item.height = 28;
            Item.UseSound = SoundID.Item3;
        }

        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.Bottle, 10)
                .AddIngredient(ItemID.Grapes)
                .AddTile(TileID.Kegs)
                .Register();
        }
    }
}
