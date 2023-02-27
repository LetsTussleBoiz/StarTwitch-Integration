using StardewValley;
using StardewValley.Menus;
using StarTwitch_Integration.Framework.Components;
using System.Collections.Generic;

namespace StarTwitch_Integration.Framework.Cheats.Positive
{
    internal class MoneyControl : BaseCommand
    {
        /*********
        ** Private methods
        *********/
        /// <summary>Drop held item.</summary>
        public void AddMoney()
        {
            Game1.player.Money += 1000;
        }

        /// <summary>Get the option field to grant the player money.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="label">The option label text.</param>
        private ModOptionsButton GetMoneyField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => AddMoney()
            );
        }

        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return new OptionsElement[]
            {
                GetMoneyField(context, I18n.Give_Money())
            };
        }
    }
}
