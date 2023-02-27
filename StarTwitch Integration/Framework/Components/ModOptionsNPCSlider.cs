using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StarTwitch_Integration.Framework.Components
{
    /// <summary>A UI slider for setting a villager's friendship level.</summary>
    internal class ModOptionsNpcSlider : BaseOptionsElement
    {
        /*********
        ** Fields
        *********/
        /// <summary>The current number of hearts.</summary>
        private int Value;

        /// <summary>The maximum number of hearts allowed.</summary>
        private readonly int MaxValue;

        /// <summary>The portrait to display next to the slider.</summary>
        private readonly ClickableTextureComponent Mugshot;

        /// <summary>The callback to invoke when the value changes.</summary>
        private readonly Action<int> SetValue;

        /// <summary>The spritesheet position for a filled heart.</summary>
        private readonly Rectangle FilledHeart = new(211, 428, 7, 6);

        /// <summary>The spritesheet position for an empty heart.</summary>
        private readonly Rectangle EmptyHeart = new(218, 428, 7, 6);

        /// <summary>The size of one rendered heart, accounting for zoom.</summary>
        private const int HeartSize = 8 * Game1.pixelZoom;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="npc">The villager NPC represented the slider.</param>
        /// <param name="value">The current number of hearts.</param>
        /// <param name="maxValue">The maximum number of hearts.</param>
        /// <param name="isMet">Whether the player has met the NPC.</param>
        /// <param name="setValue">The callback to invoke when the value changes.</param>
        public ModOptionsNpcSlider(NPC npc, int value, int maxValue, bool isMet, Action<int> setValue)
            : base(label: npc.displayName, x: 96, y: -1, width: maxValue * HeartSize, height: 6 * Game1.pixelZoom, whichOption: 0)
        {
            SetValue = setValue;
            Mugshot = new ClickableTextureComponent("Mugshot", bounds, "", "", npc.Sprite.Texture, npc.getMugShotSourceRect(), 0.7f * Game1.pixelZoom);
            greyedOut = !isMet;
            label = npc.getName();
            Value = value;
            MaxValue = maxValue;
        }

        /// <summary>Handle the player holding the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);

            int width = bounds.Width - 5;
            float valuePosition = CommonHelper.GetRangePosition(x + (HeartSize / 2), bounds.X, bounds.X + width); // offset by half a heart, so clicking the middle of a heart selects it

            greyedOut = false;
            Value = CommonHelper.GetValueAtPosition(valuePosition, 0, MaxValue);
            SetValue(Value);
        }

        /// <summary>Handle the player clicking the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void receiveLeftClick(int x, int y)
        {
            if (greyedOut)
                return;

            base.receiveLeftClick(x, y);
            leftClickHeld(x, y);

            Game1.playSound("breathin");
        }

        /// <summary>Handle the player releasing the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void leftClickReleased(int x, int y)
        {
            base.leftClickReleased(x, y);

            Game1.playSound("drumkit6");
        }

        /// <summary>Draw the component to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="slotX">The X position at which to draw, relative to the bounds.</param>
        /// <param name="slotY">The Y position at which to draw, relative to the bounds.</param>
        /// <param name="context">The menu drawing the component.</param>
        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY, IClickableMenu? context = null)
        {
            base.draw(spriteBatch, slotX + GetOffsetX(), slotY, context);

            Color tint = greyedOut ? (Color.White * 0.5f) : Color.White;

            // draw mugshot
            Mugshot.bounds = new Rectangle(slotX + 32, slotY, Game1.tileSize, Game1.tileSize);
            Mugshot.draw(spriteBatch, tint, 0.88f);

            // draw hearts
            for (int i = 0; i < MaxValue; i++)
            {
                Rectangle heartSourceRect = i < Value
                    ? FilledHeart
                    : EmptyHeart;

                spriteBatch.Draw(Game1.mouseCursors, new Vector2(slotX + bounds.X + (i * HeartSize), slotY + bounds.Y), heartSourceRect, tint, 0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.88f);
            }
        }
    }
}
