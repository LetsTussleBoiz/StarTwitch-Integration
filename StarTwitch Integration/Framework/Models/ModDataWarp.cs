﻿using Microsoft.Xna.Framework;

namespace StarTwitch_Integration.Framework.Models
{
    /// <summary>The model for a configurable warp.</summary>
    /// <param name="DisplayText">The display text to show in the menu. This can be a translation ID, or the raw text to display.</param>
    /// <param name="Location">The warp's target location name.</param>
    /// <param name="Tile">The warp's target tile coordinate.</param>
    /// <param name="Order">The relative order in which to list it in the warp menu (default 0).</param>
    /// <param name="SpecialBehavior">The special behavior to apply.</param>
    internal record ModDataWarp(string DisplayText, string? Location, Vector2 Tile, int Order, WarpBehavior SpecialBehavior);
}