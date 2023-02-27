using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using StarTwitch_Integration.Framework;
using StarTwitch_Integration.Framework.Cheats;
using StarTwitch_Integration.Framework.Components;
using StarTwitch_Integration.Framework.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StarTwitch_Integration.Framework.Cheats.Positive;

namespace StarTwitch_Integration.Framework
{
    /// <summary>An interactive menu for configuring and toggling cheats.</summary>
    internal class DebugMenu : IClickableMenu
    {
        /*********
        ** Fields
        *********/
        /// <summary>Manages the cheat implementations.</summary>
        private readonly OutputManager Cheats;

        /// <summary>Encapsulates monitoring and logging.</summary>
        private readonly IMonitor Monitor;

        private readonly List<ClickableComponent> OptionSlots = new();
        private readonly List<OptionsElement> Options = new();
        private ClickableTextureComponent UpArrow;
        private ClickableTextureComponent DownArrow;
        private ClickableTextureComponent Scrollbar;
        private readonly List<ClickableComponent> Tabs = new();
        private ClickableComponent Title;
        private const int ItemsPerPage = 10;

        /// <summary>Whether the mod is running on Android.</summary>
        private readonly bool IsAndroid = Constants.TargetPlatform == GamePlatform.Android;

        private string HoverText = "";
        private int OptionsSlotHeld = -1;
        private int CurrentItemIndex;
        private bool IsScrolling;
        private Rectangle ScrollbarRunner;

        /// <summary>Whether the menu was opened in the current tick.</summary>
        private bool JustOpened = true;


        /*********
        ** Accessors
        *********/
        /// <summary>The currently open tab.</summary>
        public MenuTabEnum CurrentTab { get; }

        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="initialTab">The tab to display by default.</param>
        /// <param name="cheats">The cheats helper.</param>
        /// <param name="monitor">Encapsulates monitoring and logging.</param>
        /// <param name="isNewMenu">Whether to play the open-menu sound.</param>
        public DebugMenu(MenuTabEnum initialTab, OutputManager cheats, IMonitor monitor, bool isNewMenu)
        {
            Cheats = cheats;
            Monitor = monitor;
            CurrentTab = initialTab;
            ResetComponents();
            SetOptions();

            Game1.playSound(isNewMenu
                ? "bigSelect"   // menu open
                : "smallSelect" // tab select
            );

            ResetComponents();
        }

        /// <summary>Exit the menu if that's allowed for the current state.</summary>
        public void ExitIfValid()
        {
            if (readyToClose() && !GameMenu.forcePreventClose)
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect");
            }
        }

        /// <summary>Whether controller-style menus should be disabled for this menu.</summary>
        public override bool overrideSnappyMenuCursorMovementBan()
        {
            return true;
        }

        /// <summary>Handle the game window being resized.</summary>
        /// <param name="oldBounds">The previous window bounds.</param>
        /// <param name="newBounds">The new window bounds.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            ResetComponents();
        }

        /// <summary>Handle the player holding the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.leftClickHeld(x, y);
            if (IsScrolling)
            {
                int num = Scrollbar.bounds.Y;
                Scrollbar.bounds.Y = Math.Min(yPositionOnScreen + height - Game1.tileSize - Game1.pixelZoom * 3 - Scrollbar.bounds.Height, Math.Max(y, yPositionOnScreen + UpArrow.bounds.Height + Game1.pixelZoom * 5));
                CurrentItemIndex = Math.Min(Options.Count - DebugMenu.ItemsPerPage, Math.Max(0, (int)Math.Round((Options.Count - DebugMenu.ItemsPerPage) * (double)((y - ScrollbarRunner.Y) / (float)ScrollbarRunner.Height))));
                SetScrollBarToCurrentIndex();
                if (num == Scrollbar.bounds.Y)
                    return;
                Game1.playSound("shiny4");
            }
            else
            {
                if (OptionsSlotHeld == -1 || OptionsSlotHeld + CurrentItemIndex >= Options.Count)
                    return;
                Options[CurrentItemIndex + OptionsSlotHeld].leftClickHeld(x - OptionSlots[OptionsSlotHeld].bounds.X, y - OptionSlots[OptionsSlotHeld].bounds.Y);
            }
        }

        /// <summary>Handle the player pressing a keyboard button.</summary>
        /// <param name="key">The key that was pressed.</param>
        public override void receiveKeyPress(Keys key)
        {
            // exit menu
            if (Game1.options.menuButton.Contains(new InputButton(key)) && !IsPressNewKeyActive())
                ExitIfValid();

            // send key to active option
            else
                GetActiveOption()?.receiveKeyPress(key);
        }

        /// <summary>Handle the player pressing a controller button.</summary>
        /// <param name="key">The key that was pressed.</param>
        public override void receiveGamePadButton(Buttons key)
        {
            // navigate tabs
            if (key is (Buttons.LeftShoulder or Buttons.RightShoulder) && !IsPressNewKeyActive())
            {
                // rotate tab index
                int index = Tabs.FindIndex(p => p.name == CurrentTab.ToString());
                if (key == Buttons.LeftShoulder)
                    index--;
                if (key == Buttons.RightShoulder)
                    index++;

                if (index >= Tabs.Count)
                    index = 0;
                if (index < 0)
                    index = Tabs.Count - 1;

                // open menu with new index
                MenuTabEnum tabID = GetTabID(Tabs[index]);
                Game1.activeClickableMenu = new DebugMenu(tabID, Cheats, Monitor, isNewMenu: false);
            }

            // send to active menu
            (GetActiveOption() as BaseOptionsElement)?.ReceiveButtonPress(key.ToSButton());
        }

        /// <summary>Handle the player scrolling the mouse wheel.</summary>
        /// <param name="direction">The scroll direction.</param>
        public override void receiveScrollWheelAction(int direction)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.receiveScrollWheelAction(direction);
            if (direction > 0 && CurrentItemIndex > 0)
                UpArrowPressed();
            else
            {
                if (direction >= 0 || CurrentItemIndex >= Math.Max(0, Options.Count - DebugMenu.ItemsPerPage))
                    return;
                DownArrowPressed();
            }
        }

        /// <summary>Handle the player releasing the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.releaseLeftClick(x, y);
            if (OptionsSlotHeld != -1 && OptionsSlotHeld + CurrentItemIndex < Options.Count)
                Options[CurrentItemIndex + OptionsSlotHeld].leftClickReleased(x - OptionSlots[OptionsSlotHeld].bounds.X, y - OptionSlots[OptionsSlotHeld].bounds.Y);
            OptionsSlotHeld = -1;
            IsScrolling = false;
        }

        /// <summary>Handle the player clicking the left mouse button.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        /// <param name="playSound">Whether to play a sound if needed.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            if (GameMenu.forcePreventClose)
                return;
            base.receiveLeftClick(x, y, playSound);

            if (DownArrow.containsPoint(x, y) && CurrentItemIndex < Math.Max(0, Options.Count - DebugMenu.ItemsPerPage))
            {
                DownArrowPressed();
                Game1.playSound("shwip");
            }
            else if (UpArrow.containsPoint(x, y) && CurrentItemIndex > 0)
            {
                UpArrowPressed();
                Game1.playSound("shwip");
            }
            else if (Scrollbar.containsPoint(x, y))
                IsScrolling = true;
            else if (!DownArrow.containsPoint(x, y) && x > xPositionOnScreen + width && (x < xPositionOnScreen + width + Game1.tileSize * 2 && y > yPositionOnScreen) && y < yPositionOnScreen + height)
            {
                IsScrolling = true;
                leftClickHeld(x, y);
                releaseLeftClick(x, y);
            }
            CurrentItemIndex = Math.Max(0, Math.Min(Options.Count - DebugMenu.ItemsPerPage, CurrentItemIndex));
            for (int index = 0; index < OptionSlots.Count; ++index)
            {
                if (OptionSlots[index].bounds.Contains(x, y) && CurrentItemIndex + index < Options.Count && Options[CurrentItemIndex + index].bounds.Contains(x - OptionSlots[index].bounds.X, y - OptionSlots[index].bounds.Y - 5))
                {
                    Options[CurrentItemIndex + index].receiveLeftClick(x - OptionSlots[index].bounds.X, y - OptionSlots[index].bounds.Y + 5);
                    OptionsSlotHeld = index;
                    break;
                }
            }

            foreach (ClickableComponent tab in Tabs)
            {
                if (tab.bounds.Contains(x, y))
                {
                    MenuTabEnum tabID = GetTabID(tab);
                    Game1.activeClickableMenu = new DebugMenu(tabID, Cheats, Monitor, isNewMenu: false);
                    break;
                }
            }
        }

        /// <summary>Handle the player hovering the cursor over the menu.</summary>
        /// <param name="x">The cursor's X pixel position.</param>
        /// <param name="y">The cursor's Y pixel position.</param>
        public override void performHoverAction(int x, int y)
        {
            if (GameMenu.forcePreventClose)
                return;
            HoverText = "";
            UpArrow.tryHover(x, y);
            DownArrow.tryHover(x, y);
            Scrollbar.tryHover(x, y);
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="spriteBatch">The sprite batch being drawn.</param>
        public override void draw(SpriteBatch spriteBatch)
        {
            if (!Game1.options.showMenuBackground)
                spriteBatch.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.4f);
            base.draw(spriteBatch);

            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);
            CommonHelper.DrawTab(Title.bounds.X, Title.bounds.Y, Game1.dialogueFont, Title.name, 1);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
            for (int index = 0; index < OptionSlots.Count; ++index)
            {
                if (CurrentItemIndex >= 0 && CurrentItemIndex + index < Options.Count)
                    Options[CurrentItemIndex + index].draw(spriteBatch, OptionSlots[index].bounds.X, OptionSlots[index].bounds.Y + 5);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (!GameMenu.forcePreventClose)
            {
                foreach (ClickableComponent tab in Tabs)
                {
                    MenuTabEnum tabID = GetTabID(tab);
                    CommonHelper.DrawTab(tab.bounds.X + tab.bounds.Width, tab.bounds.Y, Game1.smallFont, tab.label, 2, CurrentTab == tabID ? 1F : 0.7F);
                }

                UpArrow.draw(spriteBatch);
                DownArrow.draw(spriteBatch);
                if (Options.Count > DebugMenu.ItemsPerPage)
                {
                    IClickableMenu.drawTextureBox(spriteBatch, Game1.mouseCursors, new Rectangle(403, 383, 6, 6), ScrollbarRunner.X, ScrollbarRunner.Y, ScrollbarRunner.Width, ScrollbarRunner.Height, Color.White, Game1.pixelZoom, false);
                    Scrollbar.draw(spriteBatch);
                }
            }
            if (HoverText != "")
                IClickableMenu.drawHoverText(spriteBatch, HoverText, Game1.smallFont);

            if (!Game1.options.hardwareCursor && !IsAndroid)
                spriteBatch.Draw(Game1.mouseCursors, new Vector2(Game1.getOldMouseX(), Game1.getOldMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.gamepadControls ? 44 : 0, 16, 16), Color.White, 0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);

            // reinitialize the UI to fix Android pinch-zoom scaling issues
            if (JustOpened)
            {
                JustOpened = false;
                if (IsAndroid)
                    ResetComponents();
            }
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Initialize or reinitialize the UI components.</summary>
        [MemberNotNull(nameof(DebugMenu.DownArrow), nameof(DebugMenu.Scrollbar), nameof(DebugMenu.ScrollbarRunner), nameof(DebugMenu.Title), nameof(DebugMenu.UpArrow))]
        private void ResetComponents()
        {
            // set dimensions
            width = (IsAndroid ? 750 : 800) + IClickableMenu.borderWidth * 2;
            height = (IsAndroid ? 550 : 600) + IClickableMenu.borderWidth * 2;
            xPositionOnScreen = Game1.uiViewport.Width / 2 - (width - (int)(Game1.tileSize * 2.4f)) / 2;
            yPositionOnScreen = Game1.uiViewport.Height / 2 - height / 2;

            // show close button on Android
            if (IsAndroid)
                initializeUpperRightCloseButton();

            // add title
            Title = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2, yPositionOnScreen, Game1.tileSize * 4, Game1.tileSize), I18n.ModName());

            // add tabs
            {
                int i = 0;
                int labelX = (int)(xPositionOnScreen - Game1.tileSize * 4.8f);
                int labelY = (int)(yPositionOnScreen + Game1.tileSize * (IsAndroid ? 1.25f : 1.5f));
                int labelHeight = (int)(Game1.tileSize * 0.9F);

                Tabs.Clear();
                Tabs.AddRange(new[]
                {
                    new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTabEnum.Twitch.ToString(), I18n.Tabs_Twitch()),
                    new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTabEnum.Debug.ToString(), I18n.Tabs_Debug()),
                    new ClickableComponent(new Rectangle(labelX, labelY + labelHeight * i++, Game1.tileSize * 5, Game1.tileSize), MenuTabEnum.Settings.ToString(), I18n.Tabs_Settings())
                });
            }

            // add scroll UI
            int scrollbarOffset = Game1.tileSize * (IsAndroid ? 1 : 4) / 16;
            UpArrow = new ClickableTextureComponent("up-arrow", new Rectangle(xPositionOnScreen + width + scrollbarOffset, yPositionOnScreen + Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 459, 11, 12), Game1.pixelZoom);
            DownArrow = new ClickableTextureComponent("down-arrow", new Rectangle(xPositionOnScreen + width + scrollbarOffset, yPositionOnScreen + height - Game1.tileSize, 11 * Game1.pixelZoom, 12 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(421, 472, 11, 12), Game1.pixelZoom);
            Scrollbar = new ClickableTextureComponent("scrollbar", new Rectangle(UpArrow.bounds.X + Game1.pixelZoom * 3, UpArrow.bounds.Y + UpArrow.bounds.Height + Game1.pixelZoom, 6 * Game1.pixelZoom, 10 * Game1.pixelZoom), "", "", Game1.mouseCursors, new Rectangle(435, 463, 6, 10), Game1.pixelZoom);
            ScrollbarRunner = new Rectangle(Scrollbar.bounds.X, UpArrow.bounds.Y + UpArrow.bounds.Height + Game1.pixelZoom, Scrollbar.bounds.Width, height - Game1.tileSize * 2 - UpArrow.bounds.Height - Game1.pixelZoom * 2);
            SetScrollBarToCurrentIndex();

            // add option slots
            OptionSlots.Clear();
            for (int i = 0; i < DebugMenu.ItemsPerPage; i++)
                OptionSlots.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + Game1.tileSize / 4, yPositionOnScreen + Game1.tileSize * 5 / 4 + Game1.pixelZoom + i * ((height - Game1.tileSize * 2) / DebugMenu.ItemsPerPage), width - Game1.tileSize / 2, (height - Game1.tileSize * 2) / DebugMenu.ItemsPerPage + Game1.pixelZoom), string.Concat(i)));
        }

        /// <summary>Set the options to display.</summary>
        private void SetOptions()
        {
            OutputManager cheats = Cheats;
            ContextMeta context = cheats.Context;
            ModConfig config = context.Config;

            cheats.Context.SlotWidth = OptionSlots[0].bounds.Width;

            Options.Clear();
            switch (CurrentTab)
            {
                case MenuTabEnum.Twitch:
                    break;
                case MenuTabEnum.Debug:
                    // Debug Options
                    AddOptions(
                        $"{I18n.Debug_Title()}:"
                    );

                    // Weather
                    AddOptions(
                        $"{I18n.Weather_Title()}:",
                        cheats.SetWeatherForTomorrow
                    );

                    // Hearts
                    AddOptions(
                        $"{I18n.Relationships_Title()}:",
                        cheats.Hearts
                    );

                    // Monster
                    AddOptions(
                        $"{I18n.Monster_Title()}:",
                        cheats.Monsters
                    ) ;

                    // Chat
                    AddOptions(
                        $"{I18n.Chat_Title()}:",
                        cheats.Chat
                    );

                    // Drop
                    AddOptions(
                        $"{I18n.Drop_Title()}:",
                        cheats.DropItem
                    );

                    // Drop
                    AddOptions(
                        $"{I18n.Use_Title()}:",
                        cheats.UseItem
                    );

                    // Confuse
                    AddOptions(
                        $"{I18n.Confuse_Title()}:",
                        cheats.Confuse
                    );

                    // Lightning
                    AddOptions(
                        $"{I18n.Lightning_Title()}:",
                        cheats.Lightning
                    );

                    // Money
                    AddOptions(
                        $"{I18n.Money_Title()}:",
                        cheats.Money
                    );

                    // Buffs
                    AddOptions(
                        $"{I18n.Buffs_Title()}:",
                        cheats.Buffs
                    );

                    // Letters
                    AddOptions(
                        $"{I18n.Letter_Title()}:",
                        cheats.Letters
                    );
                    break;

                case MenuTabEnum.Settings:
                    break;
            }
            SetScrollBarToCurrentIndex();
        }

        /// <summary>Whether any button bind control is active and listening for input.</summary>
        private bool IsPressNewKeyActive()
        {
            return Options.Any(p => p is ModOptionsKeyListener { IsListening: true });
        }

        /// <summary>Get the currently active option, if any.</summary>
        private OptionsElement? GetActiveOption()
        {
            if (OptionsSlotHeld == -1)
                return null;

            int index = CurrentItemIndex + OptionsSlotHeld;
            return index < Options.Count
                ? Options[index]
                : null;
        }

        /// <summary>Get the first button in a key binding, if any.</summary>
        /// <param name="keybindList">The key binding list.</param>
        private SButton GetSingleButton(KeybindList keybindList)
        {
            foreach (Keybind keybind in keybindList.Keybinds)
            {
                if (keybind.IsBound)
                    return keybind.Buttons.First();
            }

            return SButton.None;
        }

        /// <summary>Add a section title to the options list.</summary>
        /// <param name="title">The section title.</param>
        private void AddTitle(string title)
        {
            Options.Add(new OptionsElement(title));
        }

        /// <summary>Add descriptive text that may extend onto multiple lines if it's too long.</summary>
        /// <param name="text">The text to render.</param>
        private void AddDescription(string text)
        {
            // get text lines
            int maxWidth = width - Game1.tileSize - 10;

            foreach (string originalLine in text.Replace("\r\n", "\n").Split('\n'))
            {
                string line = "";
                foreach (string word in originalLine.Split(' '))
                {
                    if (line == "")
                        line = word;
                    else if (Game1.smallFont.MeasureString(line + " " + word).X <= maxWidth)
                        line += " " + word;
                    else
                    {
                        Options.Add(new DescriptionElement(line));
                        line = word;
                    }
                }
                if (line != "")
                    Options.Add(new DescriptionElement(line));
            }
        }

        /// <summary>Add fields to the options list.</summary>
        /// <param name="fields">The fields to add.</param>
        private void AddOptions(params OptionsElement[] fields)
        {
            Options.AddRange(fields);
        }

        /// <summary>Add cheats to the options list.</summary>
        /// <param name="cheats">The cheats to add.</param>
        private void AddOptions(params OutputInterface[] cheats)
        {
            foreach (OptionsElement field in cheats.SelectMany(p => p.GetFields(Cheats.Context)))
                Options.Add(field);
        }

        /// <summary>Add cheats to the options list.</summary>
        /// <param name="title">The section title.</param>
        /// <param name="cheats">The cheats to add.</param>
        private void AddOptions(string title, params OutputInterface[] cheats)
        {
            AddTitle(title);
            AddOptions(cheats);
        }

        /// <summary>Reset all controls to their default value.</summary>
        private void ResetControls()
        {
            ModConfig config = Cheats.Context.Config;

            config.OpenMenuKey = ModConfig.Defaults.OpenMenuKey;
            config.PauseModKey = ModConfig.Defaults.PauseModKey;

            Game1.playSound("bigDeSelect");

            SetOptions();
        }

        private void SetScrollBarToCurrentIndex()
        {
            if (!Options.Any())
                return;
            Scrollbar.bounds.Y = ScrollbarRunner.Height / Math.Max(1, Options.Count - DebugMenu.ItemsPerPage + 1) * CurrentItemIndex + UpArrow.bounds.Bottom + Game1.pixelZoom;
            if (CurrentItemIndex != Options.Count - DebugMenu.ItemsPerPage)
                return;
            Scrollbar.bounds.Y = DownArrow.bounds.Y - Scrollbar.bounds.Height - Game1.pixelZoom;
        }

        private void DownArrowPressed()
        {
            DownArrow.scale = DownArrow.baseScale;
            ++CurrentItemIndex;
            SetScrollBarToCurrentIndex();
        }

        private void UpArrowPressed()
        {
            UpArrow.scale = UpArrow.baseScale;
            --CurrentItemIndex;
            SetScrollBarToCurrentIndex();
        }

        /// <summary>Get the tab constant represented by a tab component.</summary>
        /// <param name="tab">The component to check.</param>
        private MenuTabEnum GetTabID(ClickableComponent tab)
        {
            if (!Enum.TryParse(tab.name, out MenuTabEnum tabID))
                throw new InvalidOperationException($"Couldn't parse tab name '{tab.name}'.");
            return tabID;
        }
    }
}
