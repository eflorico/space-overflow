using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow.UI
{
    public class StackPanel : UIElement
    {
        public StackPanel() : base() { }
        public Orientation Orientation { get; set; }
        public Texture2D Split { get; set; }
        public int Spacing { get; set; }

        public override void Arrange() {
            var position = this.Position + new Vector2(this.Padding.Left, this.Padding.Top);

            foreach (var child in this.Children) {
                child.Position = position;
                child.Arrange();

                if (this.Orientation == Orientation.Horizontal)
                    position.X += child.Measure().X + (this.Split != null ? this.Split.Width : 0) + this.Spacing;
                else 
                    position.Y += child.Measure().Y + (this.Split != null ? this.Split.Height : 0) + this.Spacing;
            }
        }

        public override Vector2 Measure() {
            var size = this.Size;

            if ((size.X < 0 || size.Y < 0) && this.Children.Count > 0) {
                var lastChild = this.Children.Last();

                if (size.X < 0) size.X = lastChild.Bounds.Right - this.Position.X;
                if (size.Y < 0) size.Y = lastChild.Bounds.Bottom - this.Position.Y;
            }
            
            size += this.Padding;

            return size;
        }

        public override void DrawTo(SpriteBatch target) {
            var rect = this.Bounds;

            base.DrawTo(target);

            if (this.Split != null)
                foreach (var child in this.Children.Take(this.Children.Count - 1)) {
                    var splitRect = this.Orientation == Orientation.Horizontal ?
                        new Rectangle(child.Bounds.Right, child.Bounds.Y, this.Split.Width, child.Bounds.Height) :
                        new Rectangle(child.Bounds.X, child.Bounds.Bottom, child.Bounds.Width, this.Split.Height);

                    target.Draw(this.Split, splitRect, Color.White);
                }       
        }
    }

    public enum Orientation
    {
        Horizontal,
        Vertical
    }
}
