using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StarTwitch_Integration.Framework.Components
{
    /// <summary>A UI slider for selecting from a range of values.</summary>
    internal class ModOptionsSlider : BaseOptionsElement
    {
        /*********
        ** Fields
        *********/
        /// <summary>The field label.</summary>
        private readonly string Label;

        /// <summary>The callback to invoke when the value changes.</summary>
        private readonly Action<int> SetValue;

        /// <summary>The minimum value that can be selected using the field.</summary>
        private readonly int MinValue;

        /// <summary>The maximum value that can be selected using the field.</summary>
        private readonly int MaxValue;

        /// <summary>The current value.</summary>
        private int Value;

        /// <summary>The value's position in the range of values between <see cref="MinValue"/> and <see cref="MaxValue"/>.</summary>
        private float ValuePosition;

        /// <summary>Whether the slider should be disabled.</summary>
        private readonly Func<bool> IsDisabled;

        /// <summary>Format the display label.</summary>
        private readonly Func<int, string> Format;

        /// <summary>The pixel width of the slider area.</summary>
        private int PixelWidth => bounds.Width - 10 * Game1.pixelZoom;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="label">The field label.</param>
        /// <param name="value">The initial value.</param>
        /// <param name="maxValue">The maximum value that can be selected using the field.</param>
        /// <param name="setValue">The callback to invoke when the value changes.</param>
        /// <param name="minValue">The minimum value that can be selected using the field.</param>
        /// <param name="disabled">Whether the slider should be disabled.</param>
        /// <param name="format">Format the display label.</param>
        /// <param name="width">The field width.</param>
        public ModOptionsSlider(string label, int value, int maxValue, Action<int> setValue, int minValue = 0, Func<bool>? disabled = null, Func<int, string>? format = null, int width = 48)
            : base(label, -1, -1, width * Game1.pixelZoom, 6 * Game1.pixelZoom, 0)
        {
            Label = label;
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
            SetValue = setValue;
            IsDisabled = disabled ?? (() => false);
            Format = format ?? (v => v.ToString());

            ValuePosition = GetRangePosition();
        }

        /// <summary>Handle the player holding the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void leftClickHeld(int x, int y)
        {
            if (greyedOut)
                return;

            base.leftClickHeld(x, y);

            ValuePosition = CommonHelper.GetRangePosition(x, bounds.X, bounds.X + PixelWidth);
            Value = CommonHelper.GetValueAtPosition(ValuePosition, MinValue, MaxValue);
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
            ValuePosition = GetRangePosition(); // snap to value position
            SetValue(Value);

            Game1.playSound("drumkit6");
        }

        /// <summary>Draw the component to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        /// <param name="slotX">The X position at which to draw, relative to the bounds.</param>
        /// <param name="slotY">The Y position at which to draw, relative to the bounds.</param>
        /// <param name="context">The menu drawing the component.</param>
        public override void draw(SpriteBatch spriteBatch, int slotX, int slotY, IClickableMenu? context = null)
        {
            label = $"{Label}: {Format(Value)}";
            greyedOut = IsDisabled();

            base.draw(spriteBatch, slotX + GetOffsetX(), slotY, context);

            int sliderOffsetX = CommonHelper.GetValueAtPosition(ValuePosition, 0, PixelWidth);
            IClickableMenu.drawTextureBox(spriteBatch, Game1.mouseCursors, OptionsSlider.sliderBGSource, slotX + bounds.X, slotY + bounds.Y, bounds.Width, bounds.Height, Color.White, Game1.pixelZoom, false);
            spriteBatch.Draw(Game1.mouseCursors, new Vector2(slotX + bounds.X + sliderOffsetX, slotY + bounds.Y), OptionsSlider.sliderButtonRect, Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, 0.9f);
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Get the <see cref="Value"/>'s fractional position, as a value between 0 (<see cref="MinValue"/>) and 1 (<see cref="MaxValue"/>).</summary>
        private float GetRangePosition()
        {
            return CommonHelper.GetRangePosition(Value, MinValue, MaxValue);
        }
    }
}