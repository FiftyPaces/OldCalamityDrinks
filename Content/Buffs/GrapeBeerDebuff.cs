using OldCalamityDrinks.Content.Items;
using OldCalamityDrinks.Content.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OldCalamityDrinks.Content.Buffs
{
    public class GrapeBeerDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var modPlayer = player.GetModPlayer<GrapeBeerPlayer>();
            modPlayer.grapeBeer = true;
            modPlayer.critDamage -= GrapeBeer.CritLoss * 0.01f;
        }
    }
}
