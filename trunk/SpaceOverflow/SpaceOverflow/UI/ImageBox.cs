using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceOverflow.UI
{
    public class ImageBox : UIElement
    {
        public Texture2D Image { get; set; }
        public float Rotation { get; set; }

        protected override void DrawOverride(SpriteBatch target) {
            var globalTopLeft = new Vector2(this.Position.X + this.Padding.Left, this.Position.Y + this.Padding.Top);
            var localCenter = new Vector2(this.Image.Width /2, this.Image.Height / 2);
            target.Draw(this.Image, globalTopLeft + localCenter,
                null, Color.White, this.Rotation, localCenter, 1f, SpriteEffects.None, 0);
        }

        public override Microsoft.Xna.Framework.Vector2 Measure() {
            return new Vector2(this.Image.Width, this.Image.Height) + this.Padding;
        }
    }
}
