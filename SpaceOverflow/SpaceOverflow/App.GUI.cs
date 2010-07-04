﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpaceOverflow.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceOverflow.Effects;

namespace SpaceOverflow
{
	partial class App
	{
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

            this.UIManager = new UIManager(this.ToolBar);

            //Browse or Search
            this.RequestTypeButton = new SplitButton();
            this.ToolBar.AddChild(this.RequestTypeButton);

            //Browse
            this.BrowseButton = new Button() { Text = "Browse" };
            this.RequestTypeButton.AddItem(this.BrowseButton);

            //Search
            this.SearchButton = new Button() { Text = "Search" };
            this.RequestTypeButton.AddItem(this.SearchButton);

            //Order
            this.BrowseOptions = new SplitButton();
            this.ToolBar.AddChild(this.BrowseOptions);

            //Browse options
            this.BrowseOptions = new SplitButton();
            this.ToolBar.AddChild(this.BrowseOptions);

            this.CreationButton = new Button() { Text = "Newest" };
            this.BrowseOptions.AddItem(this.CreationButton);

            this.FeaturedButton = new Button() { Text = "Featured" };
            this.BrowseOptions.AddItem(this.FeaturedButton);

            this.HotButton = new Button() { Text = "Hot" };
            this.BrowseOptions.AddItem(this.HotButton);

            this.VotesButton = new Button() { Text = "Votes" };
            this.BrowseOptions.AddItem(this.VotesButton);

            this.ActiveButton = new Button() { Text = "Active" };
            this.BrowseOptions.AddItem(this.ActiveButton);

            //Search options
            this.SearchOptions = new StackPanel();

            //Textbox
            this.SearchBox = new TextBox() {
                Padding = new Thickness(12, 10, 0, 10),
                Size = new Vector2(140, 28),
                Caret = this.Caret,
                TextShadow = new TextShadow() {
                    Opacity = 0
                },
                Foreground = new Color(60, 60, 60),
                Placeholder = "Search"
            };
            this.SearchBox.Backgrounds.AddRange(new Background[] {
                new Background(this.TextBoxRoundedEdge, BackgroundPosition.Left),
                new Background(this.TextBoxEdge, BackgroundPosition.Right),
                new Background(this.TextBoxBackground)
            });
            this.SearchOptions.AddChild(this.SearchBox);

            //Search picker
            this.SearchPicker = new DropDownButton();
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
            this.SearchOptions.AddChild(this.SearchPicker);

            //Search picker items
            this.ByActivityButton = new Button() { Text = "User's activity" };
            //this.SearchPicker.AddItem(this.ByActivityButton);

            this.ByAuthorButton = new Button() { Text = "By author" };
            this.SearchPicker.AddItem(this.ByAuthorButton);

            this.InQuestionsButton = new Button() { Text = "In questions" };
            this.SearchPicker.AddItem(this.InQuestionsButton);

            //Selected search type
            this.SearchPicker.SelectedItem = this.InQuestionsButton;

            //Source
            this.SourceButton = new DropDownButton();
            this.SourceButton.Backgrounds.AddRange(new Background[]{
                new Background(this.ButtonEdge, BackgroundPosition.Left),
                new Background(this.ButtonEdge, BackgroundPosition.Right, SpriteEffects.FlipHorizontally),
                new Background(this.ButtonBackground),
                new Background(this.ButtonIndicator, BackgroundPosition.Center)
            });
            this.SourceButton.Button.Padding = new Thickness(12, 10, 0, 10);
            this.SourceButton.Button.Size = new Vector2(-1, 28);
            this.SourceButton.DropDownMenu.Backgrounds.AddRange(new Background[]{
                new Background(this.DropDownEdge, BackgroundPosition.Left),
                new Background(this.DropDownEdge, BackgroundPosition.Right, SpriteEffects.FlipHorizontally),
                new Background(this.DropDownBackground)
            });
            this.SourceButton.DropDownMenu.Split = this.DropDownSplit;
            this.SourceButton.DropDownMenu.Padding = new Thickness(1);
            this.ToolBar.AddChild(this.SourceButton);

            //Sources
            this.MetaButton = new Button() { Text = "Meta" };
            this.SourceButton.AddItem(this.MetaButton);

            this.StackAppsButton = new Button() { Text = "Stack Apps" };
            this.SourceButton.AddItem(this.StackAppsButton);

            this.SuperUserButton = new Button() { Text = "Super User" };
            this.SourceButton.AddItem(this.SuperUserButton);

            this.ServerFaultButton = new Button() { Text = "Server Fault" };
            this.SourceButton.AddItem(this.ServerFaultButton);

            this.StackOverflowButton = new Button() { Text = "Stack Overflow" };
            this.SourceButton.AddItem(this.StackOverflowButton);

            //Loading
            this.ProgressLabel = new Button() {
                Text = "Ready",
                TextShadow = new TextShadow() {
                    Opacity = 0
                },
                Font = this.SmallUIFont,
                Padding = new Thickness(14, 9, 0, 9)
            };
            this.ToolBar.AddChild(this.ProgressLabel);

            this.ProgressIndicator = new ImageBox() {
                Image = this.Wheel,
                Padding = new Thickness(14, 0, 0, 0),
                IsVisible = false
            };
            this.ToolBar.AddChild(this.ProgressIndicator);

            Animator.Animations.Add(new Animation(this.ProgressIndicator, "Rotation", 0f, (float)Math.PI, new TimeSpan(0, 0, 1), Interpolators.Linear) {
                Repetitions = -1
            });

            //Defaults
            this.RequestTypeButton.SelectedItem = this.BrowseButton;
            this.SourceButton.SelectedItem = this.StackOverflowButton;

            //Generic SplitButton styling
            foreach (var splitButton in new SplitButton[] { this.RequestTypeButton, this.BrowseOptions }) {
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

                splitButton.SelectedItemChanged += new EventHandler<SelectedChildChangedEventArgs>((sender, e) => {
                    if (e.OldSelectedChild != null) e.OldSelectedChild.Backgrounds.Remove(this.IndicatorBackground);
                    if (e.NewSelectedChild != null) e.NewSelectedChild.Backgrounds.Add(this.IndicatorBackground);
                    this.ReloadAndPopulate();
                });
            }

            this.RequestTypeButton.SelectedItemChanged += new EventHandler<SelectedChildChangedEventArgs>((sender, e) => {
                if (e.OldSelectedChild != e.NewSelectedChild) 
                    if (e.NewSelectedChild == this.BrowseButton) {
                        this.ToolBar.RemoveChild(this.SearchOptions);
                        this.ToolBar.InsertChild(1, this.BrowseOptions);
                    }
                    else {
                        this.ToolBar.RemoveChild(this.BrowseOptions);
                        this.ToolBar.InsertChild(1, this.SearchOptions);
                    }
            });

            this.RequestTypeButton.SelectedItem = this.BrowseButton;

            //DropDownButton item paddings
            foreach (var item in new Container[] { this.SearchPicker, this.SourceButton }.SelectMany(container => container.Items)) {
                item.Padding = new Thickness(5, 10, 0, 10);
                item.Size = new Vector2(-1, 32);
            }


            //Event handlers
            this.SearchBox.KeyPressed += new KeyEventHandler((sender, e) => {
                if (e.KeyCode == Microsoft.Xna.Framework.Input.Keys.Enter && this.SearchBox.Text != "") this.ReloadAndPopulate();
            });
            this.SearchPicker.SelectedItemChanged += new EventHandler<SelectedChildChangedEventArgs>((sender, e) => {
                if (this.SearchBox.Text != "") this.ReloadAndPopulate();
            });

            this.SourceButton.SelectedItemChanged += new EventHandler<SelectedChildChangedEventArgs>((sender, e) => this.ReloadAndPopulate());

            //Defaults
            this.BrowseOptions.SelectedItem = this.CreationButton;
            

            //Arrange and position
            this.ToolBar.Arrange();
            this.ToolBar.Position = new Vector2(0, this.Window.ClientBounds.Height - this.ToolBar.Measure().Y);

            //Browser overlay
            this.Browser = new BrowserOverlay();
        }
	}
}