using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Factoria.Common;

namespace Factoria.Content.Tiles
{
    public class Crusher : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileSolidTop[Type] = false;
            Main.tileTable[Type] = false;

            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(
                ModContent.GetInstance<TileEntities.Crusher>().Hook_AfterPlacement,
                -1,
                0,
                false
            );
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.addTile(Type);
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            ModContent.GetInstance<TileEntities.Crusher>().Kill(i, j);
        }

        public override bool RightClick(int i, int j)
        {
            if (TileUtils.TryGetTileEntityAs(i, j, out TileEntities.Crusher entity))
            {
                Main.NewText($"Crusher {entity.ID} has {entity.storedEnergy} energy stored");
                return true;
            }
            return false;
        }
    }
}
