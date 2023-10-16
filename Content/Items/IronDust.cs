using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Factoria.Content.Items
{
    public class IronDust : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 9999;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(ItemID.IronBar, 1);
            recipe.AddIngredient<IronDust>(1);
            recipe.AddTile(TileID.Furnaces);
            recipe.Register();
        }
    }
}
