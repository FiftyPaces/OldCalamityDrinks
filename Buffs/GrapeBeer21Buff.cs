using CalamityMod;
using GrapeBeer21Mod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace GrapeBeer21Mod.Buffs
{
    public class GrapeBeer21Buff : ModBuff
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
            player.GetModPlayer<GrapeBeer21Player>().grapeBeer21 = true;
            player.Calamity().critDamage -= GrapeBeer21.CritLoss * 0.01f;
        }
    }
}
