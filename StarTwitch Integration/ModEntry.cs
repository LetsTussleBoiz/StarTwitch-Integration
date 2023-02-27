using System;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StarTwitch_Integration;
using StarTwitch_Integration.Framework;
using StarTwitch_Integration.Framework.Models;
using MailFrameworkMod;

namespace StarTwitch
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        /*********
        ** Variables
        *********/
        /// <summary>Settings menu for the mod.</summary>
        private ModConfig Config = null!;

        /// <summary>Manages the cheat implementations.</summary>
        private PerScreen<OutputManager> Cheats = null!; // set in Entry

        /// <summary>The known in-game location.</summary>
        private readonly PerScreen<Lazy<GameLocation[]>> Locations = new(ModEntry.GetLocationsForCache);

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Load Config
            Config = helper.ReadConfig<ModConfig>();
            Monitor.Log($"Started with menu key {Config.OpenMenuKey}.");

            // init translations
            I18n.Init(helper.Translation);

            // load cheats
            this.ResetLocationCache();
            this.Cheats = new PerScreen<OutputManager>(() => new OutputManager(this.Config, Monitor, this.Helper.Reflection, () => this.Locations.Value.Value));

            // hook events
            helper.Events.Display.Rendered += this.OnRendered;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;

            helper.Events.GameLoop.ReturnedToTitle += this.OnReturnedToTitle;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;

            helper.Events.Input.ButtonsChanged += this.OnButtonChanged;

            helper.Events.World.LocationListChanged += this.OnLocationListChanged;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player loads a save slot.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            ResetLocationCache();
            Cheats.Value.OnSaveLoaded();
        }

        /// <summary>Raised after the player returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            ResetLocationCache();
        }

        /// <summary>Raised after a game location is added or removed.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnLocationListChanged(object? sender, LocationListChangedEventArgs e)
        {
            ResetLocationCache();
        }

        /// <summary>Raised after the player presses or releases any keys on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonChanged(object? sender, ButtonsChangedEventArgs e)
        {
            // open menu
            if (Config.OpenMenuKey.JustPressed())
            {
                if (Game1.activeClickableMenu is DebugMenu menu)
                    menu.ExitIfValid();

                else if (!Context.IsPlayerFree || Game1.currentMinigame != null)
                {
                    // Players often ask for help due to the menu not opening when expected. To
                    // simplify troubleshooting, log when the key is ignored.
                    if (Game1.currentMinigame != null)
                        Monitor.Log($"Received menu open key, but a '{Game1.currentMinigame.GetType().Name}' minigame is active.");
                    else if (Game1.eventUp)
                        Monitor.Log("Received menu open key, but an event is active.");
                    else if (Game1.activeClickableMenu != null)
                        Monitor.Log($"Received menu open key, but a '{Game1.activeClickableMenu.GetType().Name}' menu is already open.");
                    else
                        Monitor.Log("Received menu open key, but the player isn't free.");
                }

                else
                {
                    Monitor.Log("Received menu open key.");
                    OpenCheatsMenu(Config.DefaultTab, isNewMenu: true);
                }
            }

            // handle button if applicable
            Cheats.Value.OnButtonsChanged(e);
        }

        /// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRendered(object? sender, RenderedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Cheats.Value.OnRendered();
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            Cheats.Value.OnUpdateTicked(e);
        }

        /// <inheritdoc cref="IGameLoopEvents.Saving"/>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaving(object? sender, SavingEventArgs e)
        {
            Cheats.Value.OnSaving();
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // save config
            if (e.OldMenu is DebugMenu)
            {
                Helper.WriteConfig(Config);
                Cheats.Value.OnOptionsChanged();
            }
        }

      

        /// <summary>Open the cheats menu.</summary>
        /// <param name="tab">The tab to preselect.</param>
        /// <param name="isNewMenu">Whether to play the open-menu sound.</param>
        private void OpenCheatsMenu(MenuTabEnum tab, bool isNewMenu)
        {
            Game1.activeClickableMenu = new DebugMenu(tab, Cheats.Value, Monitor, isNewMenu);
        }

        /// <summary>Reset the cached location list.</summary>
        private void ResetLocationCache()
        {
            if (Locations.Value.IsValueCreated)
                Locations.Value = ModEntry.GetLocationsForCache();
        }

        /// <summary>Get a cached lookup of available locations.</summary>
        private static Lazy<GameLocation[]> GetLocationsForCache()
        {
            return new(() => Context.IsWorldReady
                ? CommonHelper.GetAllLocations().ToArray()
                : Array.Empty<GameLocation>()
            );
        }
    }
}