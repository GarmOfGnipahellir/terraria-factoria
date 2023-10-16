using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Factoria.Content.TileEntities
{
    public class Crusher : ModTileEntity
    {
        public static readonly int EnergyPerUse = 10;

        public int storedEnergy = 0;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<Tiles.Crusher>();
        }

        public override int Hook_AfterPlacement(
            int i,
            int j,
            int type,
            int style,
            int direction,
            int alternate
        )
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j);
                NetMessage.SendData(
                    MessageID.TileEntityPlacement,
                    number: i,
                    number2: j,
                    number3: Type
                );
            }

            Point16 tileOrigin = new(1, 2);
            int placedEntity = Place(i - tileOrigin.X, j - tileOrigin.Y);
            return placedEntity;
        }

        public override void OnNetPlace()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(
                    MessageID.TileEntitySharing,
                    number: ID,
                    number2: Position.X,
                    number3: Position.Y
                );
            }
        }

        public override void Update()
        {
            // TODO: Transfer energy from each connected energy source
            if (IsConnectedToGenerator())
            {
                storedEnergy++;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["storedEnergy"] = storedEnergy;
        }

        public override void LoadData(TagCompound tag)
        {
            storedEnergy = tag.GetInt("storedEnergy");
        }

        public bool IsConnectedToGenerator()
        {
            Queue<Point16> frontier = new();
            HashSet<Point16> reached = new();
            for (int i = Position.X; i < Position.X + 3; i++)
            {
                for (int j = Position.Y; j < Position.Y + 3; j++)
                {
                    if (!Main.tile[i, j].RedWire)
                    {
                        continue;
                    }

                    frontier.Enqueue(new Point16(i, j));
                    reached.Add(new Point16(i, j));
                }
            }

            while (frontier.Count > 0)
            {
                Point16 current = frontier.Dequeue();
                foreach (
                    Point16 next in new[]
                    {
                        current + new Point16(1, 0),
                        current + new Point16(0, 1),
                        current - new Point16(1, 0),
                        current - new Point16(0, 1)
                    }
                )
                {
                    Tile tile = Main.tile[next.X, next.Y];
                    if (!tile.RedWire || reached.Contains(next))
                    {
                        continue;
                    }

                    if (tile.HasTile && tile.TileType == ModContent.TileType<Tiles.Generator>())
                    {
                        return true;
                    }

                    frontier.Enqueue(next);
                    reached.Add(next);
                }
            }

            return false;
        }
    }
}
