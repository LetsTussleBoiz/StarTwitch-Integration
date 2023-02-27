using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace StarTwitch_Integration.Framework.Cheats.Negative
{
    internal class MonsterRandomizer
    {
        /*********
        ** Variables
        *********/
        /// <summary>A random number generator for obtaining mobs.</summary>
        private readonly Random random = new Random();

        public MonsterRandomizer() {}

        public Monster Randomize(int combatLevel, Microsoft.Xna.Framework.Vector2 monsterPos)
        {
            var monsterList = combatLevel switch
            {
                < 4 => new List<Monster> { new Bat(monsterPos), new GreenSlime(monsterPos), new Fly(monsterPos), new RockCrab(monsterPos) },
                >= 4 and < 8 => new List<Monster> { new Bat(monsterPos, 79), new GreenSlime(monsterPos, 79), new DustSpirit(monsterPos), new Ghost(monsterPos), new Skeleton(monsterPos) },
                >= 8 and < 10 => new List<Monster> { new Bat(monsterPos, 79), new Bat(monsterPos, 80), new GreenSlime(monsterPos, 81), new ShadowBrute(monsterPos), new LavaCrab(monsterPos),
                new MetalHead(monsterPos,81), new ShadowShaman(monsterPos), new SquidKid(monsterPos) },
                10 => new List<Monster> { new ShadowBrute(monsterPos), new Bat(monsterPos, 80), new Bat(monsterPos, 171), new Serpent(monsterPos), new BigSlime(monsterPos, 171) },
                _ => new List<Monster>()
            };
            return monsterList[random.Next(monsterList.Count - 1)];
        }
    }
}
