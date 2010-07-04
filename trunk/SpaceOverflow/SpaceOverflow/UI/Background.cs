using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceOverflow.UI
{
    /// <summary>
    /// A background drawn by a UIElement.
    /// </summary>
    public class Background
    {
        public Background(Texture2D texture)
            : this(texture, BackgroundPosition.Fill) { }

        public Background(Texture2D texture, BackgroundPosition position)
            : this(texture, position, SpriteEffects.None) { }

        public Background(Texture2D texture, BackgroundPosition position, SpriteEffects effects) {
            this.Texture = texture;
            this.Position = position;
            this.Effects = effects;
        }

        public Texture2D Texture { get; set; }
        public BackgroundPosition Position { get; set; }
        public SpriteEffects Effects { get; set; }
    }

    public enum BackgroundPosition
    {
        Top,
        Right,
        Bottom,
        Left,
        Center,
        Fill
    }
}
