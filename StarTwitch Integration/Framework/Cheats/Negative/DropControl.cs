using StardewValley;
using StardewValley.Menus;
using StarTwitch_Integration.Framework.Components;
using System.Collections.Generic;

namespace StarTwitch_Integration.Framework.Cheats.Negative
{
    internal class DropControl : BaseCommand
    {
        /*********
        ** Private methods
        *********/
        /// <summary>Drop held item.</summary>
        public void DropItem()
        {
            var itemToDrop = Game1.player.CurrentItem?.getOne();
            if (itemToDrop is null) return;
            if(itemToDrop is not Tool)
            {
                Game1.player.dropItem(itemToDrop);
                Game1.player.reduceActiveItemByOne();
            }
        }

        /// <summary>Get the option field to drop held item.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="label">The option label text.</param>
        private ModOptionsButton GetDropField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => DropItem()
            );
        }

        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return new OptionsElement[]
            {
                GetDropField(context, I18n.Drop_Item())
            };
        }
    }
}
