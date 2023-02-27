using StardewValley;
using StardewValley.Menus;
using StarTwitch_Integration.Framework.Components;
using System.Collections.Generic;

namespace StarTwitch_Integration.Framework.Cheats.Neutral
{
    internal class ChatControl : BaseCommand
    {
        public ChatControl() { }

        private void MakeChatSpeak()
        {
            Game1.chatBox.addMessage("Chat: This is a chat message from Twitch chat", Microsoft.Xna.Framework.Color.White);
        }

        private ModOptionsButton GetChatField(ContextMeta context, string label)
        {
            return new ModOptionsButton(
                label: label,
                slotWidth: context.SlotWidth,
                toggle: () => MakeChatSpeak()
            );
        }

        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return new OptionsElement[]
            {
                GetChatField(context, I18n.Chat_Speak())
            };
        }
    }
}
