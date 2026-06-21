using OldCalamityDrinks.Content.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OldCalamityDrinks.Content.Buffs
{
    public class GrapeBeerDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            var modPlayer = player.GetModPlayer<Players.GrapeBeerPlayer>();
            modPlayer.grapeBeer = true;
            modPlayer.critDamage -= GrapeBeer.CritLoss * 0.01f;
            modPlayer.buffActive = true;
        }
    }
}
