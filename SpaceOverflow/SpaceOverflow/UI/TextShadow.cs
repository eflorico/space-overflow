using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow.UI
{
    public class TextShadow
    {
        public TextShadow() {
            this.Offset = new Vector2(1, 1);
            this.Color = new Color(0, 0, 0, 0.4f);
        }
        public Vector2 Offset { get; set; }
        public Color Color { get; set; }
        public float Opacity { get; set; }

        public void DrawTo(SpriteBatch target, SpriteFont font, string text, Vector2 position, TextAlignment alignment) {
            target.DrawString(font, text, position + this.Offset, alignment, new Color(this.Color, this.Opacity));
        }
    }
}
