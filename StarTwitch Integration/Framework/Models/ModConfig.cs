using StardewModdingAPI.Utilities;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTwitch_Integration.Framework.Models
{
    /// <summary>The mod configuration model.</summary>
    internal class ModConfig
    {
        /*********
        ** Accessors
        *********/
        /// <summary>The default values.</summary>
        public static ModConfig Defaults { get; } = new();

        /****
        ** Keyboard buttons
        ****/
        /// <summary>The button which opens the menu.</summary>
        public KeybindList OpenMenuKey { get; set; } = new(SButton.G);

        /// <summary>The button which pauses the mod.</summary>
        public KeybindList PauseModKey { get; set; } = new(SButton.O);

        /****
        ** Menu settings
        ****/
        /// <summary>The tab shown by default when you open the menu.</summary>
        public MenuTabEnum DefaultTab { get; set; } = MenuTabEnum.Settings;

        /****
        ** Positive Disruptions
        ****/
        /// <summary>Chat can change the weather for the next day.</summary>
        public bool WeatherToggle { get; set; }

        /// <summary>Cooldown for Weather command (in minutes).</summary>
        public bool WeatherCooldown { get; set; }

        /// <summary>Chat can randomly increase heart levels with NPCs.</summary>
        public bool HeartUpToggle { get; set; }

        /// <summary>Cooldown for HeartUp command (in minutes).</summary>
        public bool HeartUpCooldown { get; set; }

        /// <summary>Chat can send letters with gifts.</summary>
        public bool GiftLetterToggle { get; set; }

        /// <summary>Cooldown for GiftLetter command (in minutes).</summary>
        public bool GiftLetterCooldown { get; set; }

        /// <summary>Chat can gift money.</summary>
        public bool GiftMoneyToggle { get; set; }

        /// <summary>Cooldown for GiftMoney command (in minutes).</summary>
        public bool GiftMoneyCooldown { get; set; }

        /// <summary>Chat can trigger Night Events.</summary>
        public bool NightEventToggle { get; set; }

        /// <summary>Cooldown for Night Event command (in minutes).</summary>
        public bool NightEventCooldown { get; set; }

        /// <summary>Chat can provide buffs to the player for a short time.</summary>
        public bool BuffToggle { get; set; }

        /// <summary>Cooldown for Buff command (in minutes).</summary>
        public bool BuffCooldown { get; set; }

        /****
        ** Negative Disruptions
        ****/
        /// <summary>Chat can spawn monsters.</summary>
        public bool SpawnMonsterToggle { get; set; }

        /// <summary>Cooldown for SpawnMonster command (in minutes).</summary>
        public bool SpawnMonsterCooldown { get; set; }

        /// <summary>Chat can make the player drop items.</summary>
        public bool DropItemToggle { get; set; }

        /// <summary>Cooldown for DropItem command (in minutes).</summary>
        public bool DropItemCooldown { get; set; }

        /// <summary>Chat can invert the player's controls.</summary>
        public bool InvertControlsToggle { get; set; }

        /// <summary>Cooldown for Invert Controls command (in minutes).</summary>
        public bool InvertControlsCooldown { get; set; }

        /// <summary>Chat can grant temporary debuffs. Sliding scale.</summary>
        public bool DebuffToggle { get; set; }

        /// <summary>Cooldown for Debuff command (in minutes).</summary>
        public bool DebuffCooldown { get; set; }

        /// <summary>Chat can strike the player with lightning.</summary>
        public bool LightningToggle { get; set; }

        /// <summary>Cooldown for Lightning command (in minutes).</summary>
        public bool LightningCooldown { get; set; }

        /// <summary>Chat can prevent the player from fishing for five minutes.</summary>
        public bool NoFishingToggle { get; set; }

        /// <summary>Cooldown for NoFishing command (in minutes).</summary>
        public bool NoFishingCooldown { get; set; }

        /****
        ** Neutral Disruptions
        ****/
        /// <summary>Chat messages appear in Stardew ingame chat.</summary>
        public int ChatMessagesToggle { get; set; }

        /// <summary>Chat can make the player emote.</summary>
        public bool PlayerEmoteToggle { get; set; }

        /// <summary>Cooldown for PlayerEmote command (in minutes).</summary>
        public bool PlayerEmoteCooldown { get; set; }

        /// <summary>Chat can make the player use whatever item they are holding.</summary>
        public bool UseItemToggle { get; set; }

        /// <summary>Cooldown for UseItem command (in minutes).</summary>
        public bool UseItemCooldown { get; set; }
    }
}
