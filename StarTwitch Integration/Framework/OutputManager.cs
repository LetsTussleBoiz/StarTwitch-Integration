using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;
using StarTwitch_Integration.Framework.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarTwitch_Integration.Framework.Cheats;
using StarTwitch_Integration.Framework.Cheats.Positive;
using StarTwitch_Integration.Framework.Cheats.Relationships;
using StarTwitch_Integration.Framework.Cheats.Negative;
using StarTwitch_Integration.Framework.Cheats.Neutral;
using StarTwitch_Integration.Framework.Cheats.Letters;

namespace StarTwitch_Integration.Framework
{
    internal class OutputManager
    {
        /*********
        ** Fields
        *********/
        /// <summary>The available cheat implementations.</summary>
        private readonly OutputInterface[] Cheats;

        /// <summary>The cheat implementations which should be notified of update ticks and saves.</summary>
        private readonly List<OutputInterface> NeedsUpdate = new();

        /// <summary>The cheat implementations which should be notified of user input.</summary>
        private readonly List<OutputInterface> NeedsInput = new();

        /// <summary>The cheat implementations which should be notified of render ticks.</summary>
        private readonly List<OutputInterface> NeedsRendering = new();


        /*********
        ** Accessors
        *********/
        /// <summary>The cheat context.</summary>
        public ContextMeta Context { get; }

        /****
        ** Weather
        ****/
        /// <summary>Sets the weather for tomorrow.</summary>
        public readonly OutputInterface SetWeatherForTomorrow = new WeatherControl();

        /****
        ** Relationships
        ****/
        /// <summary>Sets the heart levels for social NPCs.</summary>
        public readonly OutputInterface Hearts = new HeartsControl();

        /****
        ** Monsters
        ****/
        /// <summary>Spawns Monsters.</summary>
        public readonly OutputInterface Monsters = new MonsterControl();

        /****
        ** Chat
        ****/
        /// <summary>Make Chat Speak.</summary>
        public readonly OutputInterface Chat = new ChatControl();

        /****
        ** Drop Item
        ****/
        /// <summary>Drop held item.</summary>
        public readonly OutputInterface DropItem = new DropControl();

        /****
        ** Use Item
        ****/
        /// <summary>Use held item.</summary>
        public readonly OutputInterface UseItem = new UseControl();

        /****
        ** Confusion
        ****/
        /// <summary>Confuse the player.</summary>
        public readonly OutputInterface Confuse = new ConfusionControl();

        /****
        ** Lightning
        ****/
        /// <summary>Strike the player with lightning.</summary>
        public readonly OutputInterface Lightning;

        /****
        ** Money
        ****/
        /// <summary>Gift player money.</summary>
        public readonly OutputInterface Money = new MoneyControl();

        /****
        ** Buffs
        ****/
        /// <summary>Give players buffs or debuffs.</summary>
        public readonly OutputInterface Buffs = new BuffControl();

        /****
        ** Letters
        ****/
        /// <summary>Give players buffs or debuffs.</summary>
        public readonly OutputInterface Letters = new TwitchLetters();


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The mod configuration.</param>
        /// <param name="reflection">Simplifies access to private code.</param>
        /// <param name="getAllLocations">Get a cached list of all in-game locations.</param>
        public OutputManager(ModConfig config, IMonitor monitor, IReflectionHelper reflection, Func<IEnumerable<GameLocation>> getAllLocations)
        {
            Lightning = new LightningControl(monitor);
            Context = new ContextMeta(config, reflection, getAllLocations);

            Cheats = 
                GetType()
                .GetProperties()
                .Select(prop => prop.GetValue(this))
                .OfType<OutputInterface>()
            .ToArray();

            OnOptionsChanged();
        }

        /// <summary>Reset all tracked data.</summary>
        public void OnSaveLoaded()
        {
            foreach (OutputInterface cheat in Cheats)
                cheat.OnSaveLoaded(Context);
        }

        /// <summary>Perform any action needed after the cheat options change.</summary>
        public void OnOptionsChanged()
        {
            // update cheats
            NeedsUpdate.Clear();
            NeedsInput.Clear();
            NeedsRendering.Clear();
            foreach (OutputInterface cheat in Cheats)
            {
                cheat.OnConfig(Context, out bool needsInput, out bool needsUpdate, out bool needsRendering);
                if (needsInput)
                    NeedsInput.Add(cheat);
                if (needsUpdate)
                    NeedsUpdate.Add(cheat);
                if (needsRendering)
                    NeedsRendering.Add(cheat);
            }
        }

        /// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        public void OnRendered()
        {
            foreach (OutputInterface cheat in NeedsRendering)
                cheat.OnRendered(Context, Game1.spriteBatch);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="e">The event arguments.</param>
        public void OnUpdateTicked(UpdateTickedEventArgs e)
        {
            foreach (OutputInterface cheat in NeedsUpdate)
                cheat.OnUpdated(Context, e);
        }

        /// <summary>Raised before the game begins writing data to the save file (except the initial save creation).</summary>
        public void OnSaving()
        {
            foreach (OutputInterface cheat in NeedsUpdate)
                cheat.OnSaving(Context);
        }

        /// <summary>Raised after the player presses or releases any buttons if <see cref="OnSaveLoaded"/> indicated input was needed.</summary>
        /// <param name="e">The input event arguments.</param>
        public void OnButtonsChanged(ButtonsChangedEventArgs e)
        {
            foreach (OutputInterface cheat in NeedsInput)
                cheat.OnButtonsChanged(Context, e);
        }
    }
}
