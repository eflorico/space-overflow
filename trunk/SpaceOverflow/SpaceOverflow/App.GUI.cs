using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpaceOverflow.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceOverflow.Effects;
using StackExchange;
namespace SpaceOverflow
{
	partial class App
    {
        #region Variables
        UIManager UIManager;
            StackPanel ToolBar;
                SplitButton RequestTypeButton;
                Button BrowseButton, SearchButton;
                StackPanel SearchOptions;
                    TextBox SearchBox;
                DropDownButton ZOrderButton;
                    Dictionary<UIElement, QuestionSort> ZOrderButtons = new Dictionary<UIElement, QuestionSort>();
                DropDownButton ROrderButton;
                    Button RVotesButton, ROwnerReputationButton, RActiveButton;
                DropDownButton SearchPicker;
                    Button InQuestionsButton, ByAuthorButton, ByActivityButton;
                DropDownButton SourceButton;
                    Dictionary<UIElement, StackAPI> SourceButtons = new Dictionary<UIElement, StackAPI>();
                Button ProgressLabel;
                ImageBox ProgressIndicator;
        BrowserOverlay Browser;

        Background IndicatorBackground, TextBoxIndicatorBackground;
        Effect MaskEffect;
        Texture2D CornerMask;
        #endregion

        protected void InitializeGUI() {
            //Load resources
            this.IndicatorBackground = new Background(this.ButtonIndicator, BackgroundPosition.Center);
            this.TextBoxIndicatorBackground  =new Background(this.TextBoxIndicator, BackgroundPosition.Center);

            //Create GUI
            this.ToolBar = new StackPanel() {
                Padding = new Thickness(-4, 4, 6, 4),
                Spacing = 4,
                Font = this.UIFont,
                Foreground = new Color(252, 221, 255),
                Position = new Vector2(2, 400),
                TextShadow = new TextShadow() {
                    Color = Color.Black,
                    Offset = new Vector2(1, 1),
                    Opacity = 0.4f
                }
            };
            this.ToolBar.Backgrounds.Add(new Background(this.ToolBarBackground));
            this.UIManager = new UIManager(this.ToolBar, this.MaskEffect, this.GraphicsDevice);

            //Browse or search
            this.ToolBar.AddChild(this.RequestTypeButton = new SplitButton());
            this.RequestTypeButton.AddItem(this.BrowseButton = new Button() { Text = "Browse" });
            this.RequestTypeButton.AddItem(this.SearchButton = new Button() { Text = "Search" });
            this.RequestTypeButton.SelectedItem = this.BrowseButton;
            this.RequestTypeButton.SelectedItemChanged += new EventHandler<SelectedItemChangedEventArgs>((sender, e) => {
                if (e.OldSelectedChild != e.NewSelectedChild)
                    if (e.NewSelectedChild == this.BrowseButton) this.ToolBar.RemoveChild(this.SearchOptions);
                    else this.ToolBar.InsertChild(1, this.SearchOptions);
                this.ReloadAndPopulate();
            });
            

            //Mapping
            var zOrderPanel = new StackPanel() { Split = this.ButtonSplit };
            this.ToolBar.AddChild(zOrderPanel);
            zOrderPanel.AddChild(new Button() { Text = "Closest:" });
            zOrderPanel.AddChild(this.ZOrderButton = new DropDownButton());


            this.ZOrderButtons.Add(this.ZOrderButton.AddItem(new Button() { Text = "Newest" }), QuestionSort.Creation);
            this.ZOrderButtons.Add(this.ZOrderButton.AddItem(new Button() { Text = "Featured" }),QuestionSort.Featured);
            this.ZOrderButtons.Add(this.ZOrderButton.AddItem(new Button() { Text = "Most votes" }), QuestionSort.Votes);
            this.ZOrderButtons.Add(this.ZOrderButton.AddItem(new Button() { Text = "Hottest" }), QuestionSort.Hot);
            this.ZOrderButtons.Add(this.ZOrderButton.AddItem(new Button() { Text = "Latest activity" }), QuestionSort.Activity);

            this.ZOrderButton.SelectedItemChanged += new EventHandler<SelectedItemChangedEventArgs>((sender, e) =>
                this.ReloadAndPopulate());

            var rOrderPanel = new StackPanel();
            this.ToolBar.AddChild(rOrderPanel);
            rOrderPanel.AddChild(new Button() { Text = "Centered:" });
            rOrderPanel.AddChild(this.ROrderButton = new DropDownButton());
            this.ROrderButton.AddItem(this.RVotesButton = new Button() { Text = "Most votes" });
            this.ROrderButton.AddItem(this.ROwnerReputationButton = new Button() { Text = "Best owner rep." });
            this.ROrderButton.AddItem(this.RActiveButton = new Button() { Text = "Latest activity" });

            this.ROrderButton.SelectedItemChanged += new EventHandler<SelectedItemChangedEventArgs>((sender, e) =>
                this.ReMap());

            //Search options
            this.SearchOptions = new StackPanel();

            //Textbox
            this.SearchOptions.AddChild(this.SearchBox = new TextBox() {
                Padding = new Thickness(12, 10, 0, 10),
                Size = new Vector2(140, 28),
                Caret = this.Caret,
                TextShadow = new TextShadow() {
                    Opacity = 0
                },
                Foreground = new Color(60, 60, 60),
                Placeholder = "Search"
            });
            this.SearchBox.Backgrounds.AddRange(new Background[] {
                new Background(this.TextBoxRoundedEdge, BackgroundPosition.Left),
                new Background(this.TextBoxEdge, BackgroundPosition.Right),
                new Background(this.TextBoxBackground)
            });
            this.SearchBox.KeyPressed += new KeyEventHandler((sender, e) => {
                if (e.KeyCode == Microsoft.Xna.Framework.Input.Keys.Enter && this.SearchBox.Text != "") this.ReloadAndPopulate();
            });

            //Search picker
            this.SearchOptions.AddChild(this.SearchPicker = new DropDownButton());
            this.SearchPicker.Backgrounds.AddRange(new Background[] {
                new Background(this.ButtonEdge, BackgroundPosition.Right, SpriteEffects.FlipHorizontally),
                new Background(this.ButtonBackground),
                new Background(this.ButtonIndicator, BackgroundPosition.Center)
            });
            this.SearchPicker.Button.Padding = new Thickness(12, 10, 0, 10);
            this.SearchPicker.Button.Size = new Vector2(-1, 28);
            this.SearchPicker.DropDownMenu.Backgrounds.AddRange(new Background[]{
                new Background(this.DropDownEdge, BackgroundPosition.Left),
                new Background(this.DropDownEdge, BackgroundPosition.Right, SpriteEffects.FlipHorizontally),
                new Background(this.DropDownBackground)
            });
            this.SearchPicker.DropDownMenu.Split = this.DropDownSplit;
            this.SearchPicker.DropDownMenu.Padding = new Thickness(1);
            this.SearchPicker.SelectedItemChanged += new EventHandler<SelectedItemChangedEventArgs>((sender, e) => {
                if (this.SearchBox.Text != "") this.ReloadAndPopulate();
            });

            this.SearchPicker.AddItem(this.ByActivityButton = new Button() { Text = "User's activity" });
            this.SearchPicker.AddItem(this.ByAuthorButton = new Button() { Text = "By author" });
            this.SearchPicker.AddItem(this.InQuestionsButton = new Button() { Text = "In questions" });
            this.SearchPicker.SelectedItem = this.InQuestionsButton;

            //Source
            this.ToolBar.AddChild(this.SourceButton = new DropDownButton());
            this.SourceButton.Backgrounds.AddRange(new Background[]{
                new Background(this.ButtonEdge, BackgroundPosition.Left),
                new Background(this.ButtonEdge, BackgroundPosition.Right, SpriteEffects.FlipHorizontally),
                new Background(this.ButtonBackground),
                new Background(this.ButtonIndicator, BackgroundPosition.Center)
            });
            

            //Load cached sites
            if (Config.SiteCache.Count > 0)
                this.PopulateSources(Config.SiteCache);
            else
                new StackAuthSitesRequest().Begin(sites => {
                    sites = sites.Where(site => site.State == APIState.Normal || site.State == APIState.LinkedMeta);

                    Config.SiteCache.Clear();
                    Config.SiteCache.AddRange(sites);
                    Config.Save();

                    this.PopulateSources(sites);
                }, error => { });

            this.SourceButton.SelectedItemChanged += new EventHandler<SelectedItemChangedEventArgs>((sender, e) => this.ReloadAndPopulate());

            //Loading
            this.ToolBar.AddChild(this.ProgressLabel = new Button() {
                Text = "Ready",
                TextShadow = new TextShadow() {
                    Opacity = 0
                },
                Font = this.SmallUIFont,
                Padding = new Thickness(14, 9, 0, 9)
            });
            this.ToolBar.AddChild(this.ProgressIndicator = new ImageBox() {
                Image = this.Wheel,
                Padding = new Thickness(14, 0, 0, 0),
                IsVisible = false
            });
            this.ProgressIndicator.Animate("Rotation", 0f, (float)Math.PI, new TimeSpan(0, 0, 1)).Repetitions = -1;

            //Generic DropDownButton styling
            foreach (var dropDown in new DropDownButton[] { this.ZOrderButton, this.ROrderButton, this.SourceButton }) {
                dropDown.Backgrounds.Add(new Background(this.ButtonIndicator, BackgroundPosition.Center));
                dropDown.Button.Padding = new Thickness(12, 10, 0, 10);
                dropDown.Button.Size = new Vector2(-1, 28);
                dropDown.DropDownMenu.Backgrounds.AddRange(new Background[]{
                    new Background(this.DropDownEdge, BackgroundPosition.Left),
                    new Background(this.DropDownEdge, BackgroundPosition.Right, SpriteEffects.FlipHorizontally),
                    new Background(this.DropDownBackground)
                });
                dropDown.DropDownMenu.CornerMask = this.CornerMask;
                dropDown.DropDownMenu.Split = this.DropDownSplit;
                dropDown.DropDownMenu.Padding = new Thickness(1);
            }

            //Generic SplitButton styling
            foreach (var splitButton in new SplitButton[] { this.RequestTypeButton }) {
                splitButton.Split = this.ButtonSplit;
                splitButton.Padding = new Thickness(1);

                splitButton.Backgrounds.AddRange(new Background[]{
                    new Background(this.ButtonEdge, BackgroundPosition.Left),
                    new Background(this.ButtonEdge, BackgroundPosition.Right, SpriteEffects.FlipHorizontally),
                    new Background(this.ButtonBackground)
                });

                foreach (var item in splitButton.Items) {
                    item.Padding = new Thickness(12, 10, 0, 10); ;
                    item.Size = new Vector2(-1, 28);
                }

                splitButton.SelectedItemChanged += new EventHandler<SelectedItemChangedEventArgs>((sender, e) => {
                    if (e.OldSelectedChild != null) e.OldSelectedChild.Backgrounds.Remove(this.IndicatorBackground);
                    if (e.NewSelectedChild != null) e.NewSelectedChild.Backgrounds.Add(this.IndicatorBackground);
                });

                splitButton.SelectedItem.Backgrounds.Add(this.IndicatorBackground);
            }

            //SplitButton-like StackPanels
            foreach (var stackPanel in new StackPanel[] { zOrderPanel, rOrderPanel }) {
                stackPanel.Backgrounds.AddRange(new Background[] {
                    new Background(this.ButtonEdge, BackgroundPosition.Left),
                    new Background(this.ButtonEdge, BackgroundPosition.Right, SpriteEffects.FlipHorizontally),
                    new Background(this.ButtonBackground)
                });
                stackPanel.Split = this.ButtonSplit;
                foreach (var child in stackPanel.Children) {
                    child.Padding = new Thickness(12, 10, 0, 10); ;
                    child.Size = new Vector2(-1, 28);
                }
            }

            //DropDownButton item paddings
            foreach (var item in new Container[] { this.SearchPicker, this.ZOrderButton, this.ROrderButton }.SelectMany(container => container.Items)) {
                item.Padding = new Thickness(5, 10, 0, 10);
                item.Size = new Vector2(-1, 27);
            }

            //Browser overlay
            this.Browser = new BrowserOverlay();
        }

        protected void PopulateSources(IEnumerable<StackAPI> sites) {
            lock (this.SourceButton) {
                while (this.SourceButton.Items.Count > 0) this.SourceButton.RemoveItem(this.SourceButton.Items[0]);
                this.SourceButtons.Clear();

                foreach (var site in sites) {
                    var button = new Button() {
                        Text = site.Name,
                        Padding = new Thickness(5, 10, 0, 10),
                        Size = new Vector2(-1, 27)
                    };

                    this.SourceButtons.Add(button, site);
                    this.SourceButton.AddItem(button);
                }
            }
        }
	}
}
