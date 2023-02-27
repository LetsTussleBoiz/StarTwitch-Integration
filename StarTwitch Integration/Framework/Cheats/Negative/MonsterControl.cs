using StardewValley;
using StardewValley.Menus;
using StarTwitch_Integration.Framework.Components;
using System.Collections.Generic;

namespace StarTwitch_Integration.Framework.Cheats.Negative
{
    internal class MonsterControl : BaseCommand
    {
        /*********
        ** Private methods
        *********/
        /// <summary>Set the weather for tomorrow.</summary>
        /// <param name="weatherID">The game's weather ID.</param>
        public void SpawnMonster()
        {
            var randMonster = new MonsterRandomizer();
            // Vectors
            var left = new Microsoft.Xna.Framework.Vector2(Game1.player.Position.X - 2, Game1.player.Position.Y);
            var right = new Microsoft.Xna.Framework.Vector2(Game1.player.Position.X + 2, Game1.player.Position.Y);
            var up = new Microsoft.Xna.Framework.Vector2(Game1.player.Position.X, Game1.player.Position.Y - 2);
            var down = new Microsoft.Xna.Framework.Vector2(Game1.player.Position.X, Game1.player.Position.Y + 2);
            var monsterPos = new Microsoft.Xna.Framework.Vector2();
            if (Game1.currentLocation.isTileLocationTotallyClearAndPlaceable((int)left.X, (int)left.Y)) {
                monsterPos = left;
            } else if (Game1.currentLocation.isTileLocationTotallyClearAndPlaceable((int)right.X, (int)right.Y)) {
                monsterPos = right;
            } else if (Game1.currentLocation.isTileLocationTotallyClearAndPlaceable((int)up.X, (int)up.Y)) {
                monsterPos = up;
            } else {
                monsterPos = down;
            }
            var toReturn = randMonster.Randomize(Game1.player.combatLevel.Get(), monsterPos);
            // Check for valid placement

            // Place Monster
            Game1.currentLocation.characters.Add(toReturn);
        }

        /// <summary>Get the option field to set a weather for tomorrow.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="label">The option label text.</param>
        /// <param name="id">The game's weather ID.</param>
        private ModOptionsButton GetMonsterField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => SpawnMonster()
            );
        }

        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return new OptionsElement[]
            {
                GetMonsterField(context, I18n.Spawn_Monster())
            };
        }
    }
}
