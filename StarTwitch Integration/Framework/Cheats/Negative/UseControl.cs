using StardewValley;
using StardewValley.Menus;
using StarTwitch_Integration.Framework.Components;
using System.Collections.Generic;

namespace StarTwitch_Integration.Framework.Cheats.Negative
{
    internal class UseControl : BaseCommand
    {
        /*********
        ** Private methods
        *********/
        /// <summary>Set the weather for tomorrow.</summary>
        /// <param name="weatherID">The game's weather ID.</param>
        public void UseItem()
        {
            if(Game1.player.CurrentItem is Tool)
            {
                Game1.player.BeginUsingTool();
            }
            if (Game1.player.CurrentItem is Object heldObject && heldObject.Edibility != Object.inedible)
            {
                Game1.player.eatHeldObject();
            }
        }

        /// <summary>Get the option field to set a weather for tomorrow.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="label">The option label text.</param>
        /// <param name="id">The game's weather ID.</param>
        private ModOptionsButton GetUseField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => UseItem()
            );
        }

        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return new OptionsElement[]
            {
                GetUseField(context, I18n.Use_Item())
            };
        }
    }
}
