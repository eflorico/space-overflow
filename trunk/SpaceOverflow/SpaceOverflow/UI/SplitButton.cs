using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow.UI
{
    class SplitButton : Picker
    {
        public SplitButton()
            : base() {
            this.StackPanel = new StackPanel();
            this.AddChild(this.StackPanel);
        }

        public Orientation Orientation {
            get {
                return this.StackPanel.Orientation;
            }
            set {
                this.StackPanel.Orientation = value;
            }
        }

        public Texture2D Split {
            get {
                return this.StackPanel.Split;
            }
            set {
                this.StackPanel.Split = value;
            }
        }

        public Texture2D SelectedIndicator { get; set; }

        protected StackPanel StackPanel { get; set; }

        protected override void OnItemAdded(UIElement item) {
            base.OnItemAdded(item);
            this.StackPanel.AddChild(item);
            item.Clicked += this.ItemClicked;
        }

        protected override void OnItemRemoved(UIElement item) {
            base.OnItemRemoved(item);
            this.StackPanel.RemoveChild(item);
            item.Clicked -= this.ItemClicked;
        }

        private void ItemClicked(object sender, EventArgs e) {
            this.SelectedItem = (UIElement)sender;
        }

        public override void DrawTo(SpriteBatch target)
        {
            base.DrawBackgrounds(target);

            var rect = this.Bounds;

            foreach (var item in this.Items)
            {
                if (item == this.SelectedItem && this.SelectedIndicator != null)
                    target.Draw(this.SelectedIndicator,
                        new Vector2(item.Bounds.Center.X - this.SelectedIndicator.Width / 2, this.Position.Y),
                        Color.White);
            }

            base.DrawChildren(target);
        }

        public override void Arrange() {
            if (this.Items.Count == 0) return;

            var itemSizes = this.Items.Select(item => 
                item.Measure() - item.Padding);
            var itemWidth = itemSizes.Max(sz => sz.X);
            var itemHeight = itemSizes.Max(sz => sz.Y);

            foreach (var item in this.Items)
                item.Size = new Vector2(itemWidth, itemHeight);

            this.StackPanel.Position = this.Position;

            base.Arrange();
        }

        public override Vector2 Measure() {
            return this.StackPanel.Measure();
        } 
    }
}
