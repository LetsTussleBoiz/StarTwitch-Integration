using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StarTwitch_Integration.Framework.Components;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using static StardewValley.Farm;

namespace StarTwitch_Integration.Framework.Cheats.Negative
{
    internal class LightningControl : BaseCommand
    {

        private readonly IMonitor _monitor;

        public LightningControl(IMonitor monitor) {
            _monitor = monitor;
        }

        /*********
        ** Private methods
        *********/
        /// <summary>Strike the player with lightning.</summary>
        public void StrikePlayer()
        {
            Farm.LightningStrikeEvent luluStrike = new()
            {
                boltPosition = Game1.player.position,
                bigFlash = true,
                createBolt = true,
            };
            doLightningStrike(luluStrike);
        }

        private void doLightningStrike(LightningStrikeEvent lightning)
        {
            if (lightning.smallFlash)
            {
                if (!Game1.newDay)
                {
                    Game1.flashAlpha = (float)(0.5 + Game1.random.NextDouble());
                    if (Game1.random.NextDouble() < 0.5)
                    {
                        DelayedAction.screenFlashAfterDelay((float)(0.3 + Game1.random.NextDouble()), Game1.random.Next(500, 1000));
                    }

                    DelayedAction.playSoundAfterDelay("thunder_small", Game1.random.Next(500, 1500));
                }
            }
            else if (lightning.bigFlash && !Game1.newDay)
            {
                Game1.flashAlpha = (float)(0.5 + Game1.random.NextDouble());
                Game1.playSound("thunder");
            }
            _monitor.Log($"Bolt struck at {Game1.player.getTileLocation()}.");
            Utility.drawLightningBolt(lightning.boltPosition, Game1.currentLocation);
            Game1.player.takeDamage(15,true,null);
        }

        /// <summary>Get the option field to strike the player with lightning.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="label">The option label text.</param>
        private ModOptionsButton GetStrikeField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => StrikePlayer()
            );
        }

        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return new OptionsElement[]
            {
                GetStrikeField(context, I18n.Lightning_Strike()),
            };
        }
    }
}
