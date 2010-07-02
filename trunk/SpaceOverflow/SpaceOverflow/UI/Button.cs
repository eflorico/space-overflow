using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow.UI
{
    class Button : UIElement
    {
        public Button() {
            this.Text = "";
        }

        public string Text { get; set; }

        
        public override void Arrange() {
            foreach (var child in this.Children) {
                child.Position = new Vector2(this.Position.X + this.Padding.Left, this.Position.Y + this.Padding.Top);
                child.Arrange();
            }
        }

        public override Vector2 Measure() {
            var size = this.Size;

            if (size.X < 0 || size.Y < 0) {
                var textSize = this.Font.MeasureString(this.Text);

                if (size.X < 0) size.X = (float)Math.Ceiling(textSize.X);
                if (size.Y < 0) size.Y = (float)Math.Ceiling(textSize.Y);
            }

            size.X += this.Padding.Left + this.Padding.Right;
            size.Y += this.Padding.Top + this.Padding.Bottom;

            return size;
        }

        public override void DrawTo(SpriteBatch target) {
            base.DrawTo(target);

            var rect = this.Bounds;

            rect -= this.Padding;

            var textPosition = new Vector2(rect.Center.X, rect.Y);
            if (this.TextShadow != null) this.TextShadow.DrawTo(target, this.Font, this.Text, textPosition, TextAlignment.Center);           
            target.DrawString(this.Font, this.Text, textPosition, TextAlignment.Center, this.Foreground.Value);
        }

        public override string ToString() {
            return this.Text;
        }
    }
}