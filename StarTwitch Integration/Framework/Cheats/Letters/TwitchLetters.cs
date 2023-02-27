using System.Collections.Generic;
using MailFrameworkMod;
using StardewValley.Menus;
using StardewValley;
using StarTwitch_Integration.Framework.Components;
using System;

namespace StarTwitch_Integration.Framework.Cheats.Letters
{
    internal class TwitchLetters : BaseCommand
    {

        public void SendLetter()
        {
            List<Item> itemToSend = new() { new StardewValley.Object(334, 1) };
            Letter twitchLetter = new(Guid.NewGuid().ToString(), "This is a test.", itemToSend, letter => true, _ => { }, 0);
            MailDao.SaveLetter(twitchLetter);
        }


        private ModOptionsButton GetLetterField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => SendLetter()
            );
        }

        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return new OptionsElement[]
            {
                GetLetterField(context, I18n.Send_Letter()),
            };
        }
    }
}
