using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarTwitch_Integration.Framework.Components
{
    internal class BaseOptionsElement : OptionsElement
    {
        /*********
        ** Protected methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="label">The element label.</param>
        protected BaseOptionsElement(string label)
            : base(label) { }

        /// <summary>Construct an instance.</summary>
        /// <param name="label">The display label.</param>
        /// <param name="x">The X pixel position at which to draw the element.</param>
        /// <param name="y">The Y pixel position at which to draw the element.</param>
        /// <param name="width">The pixel width.</param>
        /// <param name="height">The pixel height.</param>
        /// <param name="whichOption">The option ID.</param>
        protected BaseOptionsElement(string label, int x, int y, int width, int height, int whichOption = -1)
            : base(label, x, y, width, height, whichOption) { }

        /// <summary>Get the X offset at which to render the element.</summary>
        protected int GetOffsetX()
        {
            // Android port doesn't consider the element width, so we do so here
            return Constants.TargetPlatform == GamePlatform.Android
                ? bounds.Width + 8
                : 0;
        }

        /// <summary>Handle the player pressing a button.</summary>
        /// <param name="button">The button that was pressed.</param>
        public virtual void ReceiveButtonPress(SButton button) { }

        /// <inheritdoc />
        public override void receiveKeyPress(Keys key)
        {
            ReceiveButtonPress(key.ToSButton());

            base.receiveKeyPress(key);
        }
    }
}
