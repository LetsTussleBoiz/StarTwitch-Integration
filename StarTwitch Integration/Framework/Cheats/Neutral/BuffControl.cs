using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StarTwitch_Integration.Framework.Components;
using System;
using System.Collections.Generic;

namespace StarTwitch_Integration.Framework.Cheats.Neutral
{
    internal class BuffControl : BaseCommand
    {
        /*********
        ** Private methods
        *********/
        /// <summary>Give the buff to the player.</summary>
        public void GrantBuff()
        {
            var randoBuff = BuffRandomizer.RandomizeBuff();
            Game1.buffsDisplay.addOtherBuff(new Buff(randoBuff.ToString(),10000,"StarTwitch",(int)randoBuff));
        }

        /// <summary>Give the buff to the player.</summary>
        public void GrantDebuff()
        {
            var randoBuff = BuffRandomizer.RandomizeDebuff();
            Game1.buffsDisplay.addOtherBuff(new Buff(randoBuff.ToString(), 10000, "StarTwitch", (int)randoBuff));
        }

        /// <summary>Get the option field to set a weather for tomorrow.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="label">The option label text.</param>
        private ModOptionsButton GetBuffField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => GrantBuff()
            );
        }
        private ModOptionsButton GetDebuffField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => GrantDebuff()
            );
        }


        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return new OptionsElement[]
            {
                GetBuffField(context, I18n.Give_Buff()),
                GetDebuffField(context, I18n.Give_Debuff()),
            };
        }
    }
}
