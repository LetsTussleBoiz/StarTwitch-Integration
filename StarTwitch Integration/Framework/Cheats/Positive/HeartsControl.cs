using System;
using System.Collections.Generic;
using System.Linq;
using StarTwitch_Integration.Framework.Components;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;
using StarTwitch_Integration.Framework;
using StarTwitch_Integration.Framework.Cheats;

namespace StarTwitch_Integration.Framework.Cheats.Relationships
{
    /// <summary>A cheat which sets the heart levels for social NPCs.</summary>
    internal class HeartsControl : BaseCommand
    {

        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        public override IEnumerable<OptionsElement> GetFields(ContextMeta context)
        {
            return SortFields(
                GetSocialCharacters()
                    .Distinct()
                    .Select(GetField)
                    .ToArray()
            );
        }

        /// <summary>Handle the cheat options being loaded or changed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="needsUpdate">Whether the cheat should be notified of game updates.</param>
        /// <param name="needsInput">Whether the cheat should be notified of button presses.</param>
        /// <param name="needsRendering">Whether the cheat should be notified of render ticks.</param>
        public override void OnConfig(ContextMeta context, out bool needsInput, out bool needsUpdate, out bool needsRendering)
        {
            needsInput = false;
            needsUpdate = false;
            needsRendering = false;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the unsorted fields to display.</summary>
        private OptionsElement GetField(NPC npc)
        {
            // get friendship info
            Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship? friendship);
            bool isSpouse = friendship?.IsMarried() ?? false;
            int curHearts = (friendship?.Points ?? 0) / NPC.friendshipPointsPerHeartLevel;
            int maxHearts = isSpouse ? 14 : NPC.maxFriendshipPoints / NPC.friendshipPointsPerHeartLevel;

            // get field
            return new ModOptionsNpcSlider(
                npc: npc,
                isMet: friendship != null,
                value: curHearts,
                maxValue: maxHearts,
                setValue: hearts => SetFriendshipHearts(npc, hearts)
            );
        }

        /// <summary>Set the friendship hearts for an NPC.</summary>
        /// <param name="npc">The NPC to change.</param>
        /// <param name="hearts">The friendship hearts to set.</param>
        private void SetFriendshipHearts(NPC npc, int hearts)
        {
            // add friendship if needed
            if (!Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship friendship))
            {
                friendship = new Friendship();
                Game1.player.friendshipData.Add(npc.Name, friendship);
                SocializeQuest? socialQuest = Game1.player.questLog.OfType<SocializeQuest>().FirstOrDefault();
                if (socialQuest != null && !socialQuest.completed.Value)
                    socialQuest.checkIfComplete(npc);
            }

            // update friendship points
            friendship.Points = hearts * NPC.friendshipPointsPerHeartLevel;
        }
    }
}