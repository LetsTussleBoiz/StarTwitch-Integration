using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StarTwitch_Integration.Framework.Components;
using System.Collections.Generic;

namespace StarTwitch_Integration.Framework.Cheats.Negative
{
    internal class ConfusionControl : BaseCommand
    {
        public InputButton[] SavedList { get; set; } = new InputButton[4];

        /*********
        ** Private methods
        *********/
        /// <summary>Invert the players movement controls.</summary>
        public void ConfusePlayer()
        {
            SavedList[0] = Game1.options.moveUpButton[0];
            SavedList[1] = Game1.options.moveDownButton[0];
            SavedList[2] = Game1.options.moveLeftButton[0];
            SavedList[3] = Game1.options.moveRightButton[0];
            Game1.options.moveUpButton[0].key = SavedList[1].key;
            Game1.options.moveDownButton[0].key = SavedList[0].key;
            Game1.options.moveLeftButton[0].key = SavedList[3].key;
            Game1.options.moveRightButton[0].key = SavedList[2].key;
        }

        /// <summary>UN-Invert the players movement controls.</summary>
        public void UnconfusePlayer()
        {
            Game1.options.moveUpButton[0].key = SavedList[0].key;
            Game1.options.moveDownButton[0].key = SavedList[1].key;
            Game1.options.moveLeftButton[0].key = SavedList[2].key;
            Game1.options.moveRightButton[0].key = SavedList[3].key;
        }

        /// <summary>Get the option field to set a weather for tomorrow.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="label">The option label text.</param>
        private ModOptionsButton GetConfuseField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => ConfusePlayer()
            );
        }

        private ModOptionsButton GetUnconfuseField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => UnconfusePlayer()
            );
        }

        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return new OptionsElement[]
            {
                GetConfuseField(context, I18n.Confuse_Player()),
                GetUnconfuseField(context, I18n.Unconfuse_Player())
            };
        }
    }
}
