using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace Factoria.Common.Systems
{
    public class PlayerSystem : ModSystem
    {
        public static readonly ItemTrader CrusherItemTrader = CreateCrusherItemTrader();

        public override void Load()
        {
            On_Player.PlaceThing += On_Player_PlaceThing;
        }

        private static void On_Player_PlaceThing(
            On_Player.orig_PlaceThing orig,
            Player self,
            ref Player.ItemCheckContext context
        )
        {
            orig(self, ref context);

            On_Player_PlaceThing_ItemInTutorialTile(self, ref context);
        }

        private static void On_Player_PlaceThing_ItemInTutorialTile(
            Player self,
            ref Player.ItemCheckContext context
        )
        {
            // This is taken from Player.PlaceThing_ItemInExtractinator
            Tile tile = Main.tile[Player.tileTargetX, Player.tileTargetY];
            Item item = self.inventory[self.selectedItem];
            if (
                !tile.HasTile
                || self.position.X / 16f
                    - (float)Player.tileRangeX
                    - (float)item.tileBoost
                    - (float)self.blockRange
                    > (float)Player.tileTargetX
                || (self.position.X + (float)self.width) / 16f
                    + (float)Player.tileRangeX
                    + (float)item.tileBoost
                    - 1f
                    + (float)self.blockRange
                    < (float)Player.tileTargetX
                || self.position.Y / 16f
                    - (float)Player.tileRangeY
                    - (float)item.tileBoost
                    - (float)self.blockRange
                    > (float)Player.tileTargetY
                || (self.position.Y + (float)self.height) / 16f
                    + (float)Player.tileRangeY
                    + (float)item.tileBoost
                    - 2f
                    + (float)self.blockRange
                    < (float)Player.tileTargetY
                || !self.ItemTimeIsZero
                || self.itemAnimation <= 0
                || !self.controlUseItem
            )
            {
                return;
            }

            if (
                tile.TileType == ModContent.TileType<Content.Tiles.Crusher>()
                && CrusherItemTrader.TryGetTradeOption(item, out ItemTrader.TradeOption option)
                && TileUtils.TryGetTileEntityAs(
                    Player.tileTargetX,
                    Player.tileTargetY,
                    out Content.TileEntities.Crusher crusherEntity
                )
                && crusherEntity.storedEnergy >= Content.TileEntities.Crusher.EnergyPerUse
            )
            {
                SoundEngine.PlaySound(SoundID.Grab);
                self.ApplyItemTime(item, 1.0f, null);
                context.SkipItemConsumption = true;
                item.stack -= option.TakingItemStack;
                if (item.stack <= 0)
                {
                    item.TurnToAir(false);
                }
                DropItemFromTutorialTile(self, option.GivingITemType, option.GivingItemStack);
                crusherEntity.storedEnergy -= Content.TileEntities.Crusher.EnergyPerUse;
            }
        }

        private static void DropItemFromTutorialTile(Player self, int itemType, int stack)
        {
            Vector2 vector = Main.ReverseGravitySupport(Main.MouseScreen, 0f) + Main.screenPosition;
            if (Main.SmartCursorIsUsed || PlayerInput.UsingGamepad)
            {
                vector = self.Center;
            }
            int number = Item.NewItem(
                self.GetSource_TileInteraction(Player.tileTargetX, Player.tileTargetY),
                (int)vector.X,
                (int)vector.Y,
                1,
                1,
                itemType,
                stack,
                false,
                -1,
                false,
                false
            );
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f, 0f, 0f, 0, 0, 0);
            }
        }

        private static ItemTrader CreateCrusherItemTrader()
        {
            ItemTrader itemTrader = new();
            itemTrader.AddOption_OneWay(
                ItemID.IronOre,
                1,
                ModContent.ItemType<Content.Items.IronDust>(),
                2
            );
            return itemTrader;
        }
    }
}
