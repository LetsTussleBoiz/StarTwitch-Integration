using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StarTwitch_Integration.Framework.Components
{
    internal class WeatherElement : BaseElement
    {
        /*********
        ** Fields
        *********/
        /// <summary>Get the current weather name.</summary>
        private readonly Func<string> CurrentWeather;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="label">The field label.</param>
        /// <param name="currentWeather">Get the current weather name.</param>
        public WeatherElement(string label, Func<string> currentWeather)
          : base(label)
        {
            CurrentWeather = currentWeather;
            whichOption = 0;
        }

        /// <summary>Draw the component to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="slotX">The X position at which to draw, relative to the bounds.</param>
        /// <param name="slotY">The Y position at which to draw, relative to the bounds.</param>
        /// <param name="context">The menu drawing the component.</param>
        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY, IClickableMenu? context = null)
        {
            string info = CurrentWeather();
            Utility.drawTextWithShadow(spriteBatch, $"{label}: {info}", Game1.dialogueFont, new Vector2(bounds.X + slotX, bounds.Y + slotY), Game1.textColor, 1f, 0.15f);
        }
    }
}
