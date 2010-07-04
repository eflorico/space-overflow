using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow.UI
{
    class DropDownButton : Picker
    {
        public DropDownButton()
            : base() {
            this.DropDownMenu = new StackPanel() {
                Orientation = Orientation.Vertical
            };
            this.AddChild(this.DropDownMenu);

            this.Button = new Button();
            this.Button.Clicked += this.OnSelectedItemClicked;
            this.AddChild(this.Button);

            this.IsDropped = false;
        }

        protected override void OnItemAdded(UIElement item) {
            base.OnItemAdded(item);
            if (this.SelectedItem != item) this.DropDownMenu.AddChild(item);
            item.Clicked += this.ItemClicked;
        }

        protected override void OnItemRemoved(UIElement item) {
            base.OnItemRemoved(item);
            this.DropDownMenu.RemoveChild(item);
            item.Clicked -= this.ItemClicked;
        }

        protected override void OnSelectedItemChanged(UIElement oldSelectedItem, UIElement newSelectedItem) {
            base.OnSelectedItemChanged(oldSelectedItem, newSelectedItem);

            if (oldSelectedItem != null) {
                this.DropDownMenu.AddChild(oldSelectedItem);
                this.DropDownMenu.Children.Remove(oldSelectedItem);
                this.DropDownMenu.Children.Insert(this.Items.IndexOf(oldSelectedItem), oldSelectedItem);
            }
            if (newSelectedItem != null) {
                this.DropDownMenu.RemoveChild(newSelectedItem);
                if (newSelectedItem is Button) this.Button.Text = (newSelectedItem as Button).Text;
            }
        }

        protected virtual void OnSelectedItemClicked(object sender, EventArgs e) {
            this.IsDropped = !this.IsDropped;
        }

        private void ItemClicked(object sender, EventArgs e) {
            var el = (sender as UIElement);
            if (el != null && el != this.SelectedItem) {
                this.SelectedItem = (UIElement)sender;
                this.IsDropped = false;
            }
        }

        public bool IsDropped {
            get { return this.DropDownMenu.IsVisible; }
            set { this.DropDownMenu.IsVisible = value; }
        }

        public Texture2D SelectedIndicator { get; set; }
        public StackPanel DropDownMenu { get; private set; }
        public Button Button { get; private set; }

        public override void Arrange() {
            this.DropDownMenu.AddChild(this.SelectedItem); //Temporarily add selected item for inheritance

            var itemSizes = this.Items.Select(item => item.Measure() - item.Padding);
            var width = itemSizes.Max(sz => sz.X);
            var height = itemSizes.Max(sz => sz.Y);

            this.DropDownMenu.RemoveChild(this.SelectedItem);

            foreach (var item in this.Items)
                item.Size = new Vector2(width, height);

            if (this.IsDropped) {
                this.DropDownMenu.Arrange();

                var dropDownSize = this.DropDownMenu.Measure();

                this.DropDownMenu.Position = new Vector2(
                    this.Bounds.Center.X - dropDownSize.X / 2,
                    this.Position.Y - 4 - dropDownSize.Y
                    );

                this.DropDownMenu.Arrange();
            }

            this.Button.Position = this.Position;
            this.Button.Size = new Vector2(width, this.Button.Size.Y);
            this.Button.Arrange();
        }

        public override Vector2 Measure() {
            return this.Button.Measure();
        }

        protected override void DrawOverride(SpriteBatch target) {
            base.DrawBackgrounds(target);

            if (this.SelectedIndicator != null)
                target.Draw(this.SelectedIndicator,
                            new Vector2(this.Button.Bounds.Center.X - this.SelectedIndicator.Width / 2, this.Button.Position.Y),
                            Color.White);

            base.DrawChildren(target);
        }

        public override void HandleMouse(MouseState mouseState, MouseState lastMouseState) {
            this.Button.HandleMouse(mouseState, lastMouseState);
            if (this.IsDropped) this.DropDownMenu.HandleMouse(mouseState, lastMouseState);
        }
    }
}
