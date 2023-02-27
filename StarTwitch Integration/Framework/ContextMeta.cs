using StardewModdingAPI;
using StardewValley;
using StarTwitch_Integration.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTwitch_Integration.Framework
{
    internal class ContextMeta
    {
        /*********
        ** Fields
        *********/
        /// <summary>Get a cached list of all in-game locations.</summary>
        private readonly Func<IEnumerable<GameLocation>> GetAllLocationsImpl;


        /*********
        ** Accessors
        *********/
        /// <summary>The mod configuration.</summary>
        public ModConfig Config { get; }

        /// <summary>Simplifies access to private code.</summary>
        public IReflectionHelper Reflection { get; }

        /// <summary>The display width of an option slot during the last cheats menu render.</summary>
        public int SlotWidth { get; set; }


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="config">The mod configuration.</param>
        /// <param name="reflection">Simplifies access to private code.</param>
        /// <param name="getAllLocations">Get a cached list of all in-game locations.</param>
        public ContextMeta(ModConfig config, IReflectionHelper reflection, Func<IEnumerable<GameLocation>> getAllLocations)
        {
            Config = config;
            Reflection = reflection;
            GetAllLocationsImpl = getAllLocations;
        }

        /// <summary>Get all in-game locations.</summary>
        public IEnumerable<GameLocation> GetAllLocations()
        {
            return GetAllLocationsImpl();
        }
    }
}
