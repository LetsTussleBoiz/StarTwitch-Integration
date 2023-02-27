using StardewValley;
using System;

namespace StarTwitch_Integration.Framework.Cheats.Neutral
{
    internal class BuffRandomizer
    {
        /*********
        ** Variables
        *********/
        /// <summary>A random number generator for obtaining buffs/debuffs.</summary>
        private static readonly Random random = new Random();

        public BuffRandomizer() {}


        public static BuffEnum RandomizeBuff()
        {
            var buffs = Enum.GetValues<BuffEnum>();
            return buffs[random.Next(buffs.Length - 1)];
        }

        public static DebuffEnum RandomizeDebuff()
        {
            var debuffs = Enum.GetValues<DebuffEnum>();
            return debuffs[random.Next(debuffs.Length - 1)];
        }
    }
}
