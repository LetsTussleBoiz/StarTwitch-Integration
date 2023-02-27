using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System;
using Microsoft.Xna.Framework;

namespace StarTwitch_Integration.Framework.Components
{
    internal class ModOptionsButton : BaseOptionsElement
    {
        /*********
        ** Fields
        *********/
        /// <summary>The action to perform when the button is toggled (or <c>null</c> to handle it manually).</summary>
        private readonly Action Toggle;

        /// <summary>The source rectangle for the 'set' button sprite.</summary>
        private readonly Rectangle SetButtonSprite = new(294, 428, 21, 11);

        /// <summary>The button area in screen pixels.</summary>
        private Rectangle SetButtonBounds;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="label">The field label.</param>
        /// <param name="slotWidth">The field width.</param>
        /// <param name="toggle">The action to perform when the button is toggled.</param>
        /// <param name="disabled">Whether the button should be disabled.</param>
        public ModOptionsButton(string label, int slotWidth, Action toggle, bool disabled = false)
          : base(label, -1, -1, slotWidth + 1, 11 * Game1.pixelZoom)
        {
            SetButtonBounds = new Rectangle(slotWidth - 28 * Game1.pixelZoom, -1 + Game1.pixelZoom * 3, 21 * Game1.pixelZoom, 11 * Game1.pixelZoom);
            Toggle = toggle;
            greyedOut = disabled;
        }

        /// <summary>Handle the player clicking the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void receiveLeftClick(int x, int y)
        {
            if (greyedOut || !SetButtonBounds.Contains(x, y))
                return;

            // callback handler
            Toggle();
        }

        /// <summary>Draw the component to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="slotX">The X position at which to draw, relative to the bounds.</param>
        /// <param name="slotY">The Y position at which to draw, relative to the bounds.</param>
        /// <param name="context">The menu drawing the component.</param>
        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY, IClickableMenu? context = null)
        {
            Utility.drawTextWithShadow(spriteBatch, label, Game1.dialogueFont, new Vector2(bounds.X + slotX, bounds.Y + slotY), greyedOut ? Game1.textColor * 0.33f : Game1.textColor, 1f, 0.15f);
            Utility.drawWithShadow(spriteBatch, Game1.mouseCursors, new Vector2(SetButtonBounds.X + slotX, SetButtonBounds.Y + slotY), SetButtonSprite, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, false, 0.15f);
        }
    }
}
