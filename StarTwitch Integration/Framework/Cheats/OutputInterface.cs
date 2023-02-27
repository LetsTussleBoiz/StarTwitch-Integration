using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTwitch_Integration.Framework.Cheats
{
    internal interface OutputInterface
    {
        /*********
        ** Methods
        *********/
        /// <summary>Get the config UI fields to show in the cheats menu.</summary>
        /// <param name="context">The cheat context.</param>
        IEnumerable<OptionsElement> GetFields(ContextMeta context);

        /// <summary>Handle the cheat options being loaded or changed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="needsUpdate">Whether the cheat should be notified of game updates and saves.</param>
        /// <param name="needsInput">Whether the cheat should be notified of button presses.</param>
        /// <param name="needsRendering">Whether the cheat should be notified of render ticks.</param>
        void OnConfig(ContextMeta context, out bool needsInput, out bool needsUpdate, out bool needsRendering);

        /// <summary>Handle the player loading a save file.</summary>
        /// <param name="context">The cheat context.</param>
        void OnSaveLoaded(ContextMeta context);

        /// <summary>Handle the player pressing or releasing any buttons if <see cref="OnSaveLoaded"/> indicated input was needed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="e">The input event arguments.</param>
        void OnButtonsChanged(ContextMeta context, ButtonsChangedEventArgs e);

        /// <summary>Handle a game update if <see cref="OnSaveLoaded"/> indicated updates were needed.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="e">The update event arguments.</param>
        void OnUpdated(ContextMeta context, UpdateTickedEventArgs e);

        /// <summary>Raised before the game begins writing data to the save file (except the initial save creation).</summary>
        /// <param name="context">The cheat context.</param>
        void OnSaving(ContextMeta context);

        /// <summary>Handle the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        /// <param name="context">The cheat context.</param>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        void OnRendered(ContextMeta context, SpriteBatch spriteBatch);
    }
}
