using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SpaceOverflow.Effects;

namespace SpaceOverflow
{
    public static class Extensions
    {
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, SpriteEffects effects) {
            spriteBatch.Draw(texture, position, null, color, 0f, new Vector2(), 1f, effects, 0);
        }

        public static void DrawString(this SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, TextAlignment alignment, Color color) {
            if (alignment == TextAlignment.Center)
                position.X -= (font.MeasureString(text) / 2).X;
            else if (alignment == TextAlignment.Right)
                position.X -= font.MeasureString(text).X;

            spriteBatch.DrawString(font, text, position, color);
        }

        public static void FillSolid(this Texture2D texture, Color color) {
            var data = new Color[texture.Width * texture.Height];
            for (var i = 0; i < data.Length; ++i)
                data[i] = color;
            texture.SetData(data);
        }

        public static Vector2 GetPosition(this MouseState mouseState) {
            return new Vector2(mouseState.X, mouseState.Y);
        }

        public static Interpolator GetInterpolator(this Curve3D curve) {
            return new Interpolator((from, to, progress) => {
                var x =  new Vector3(
                    curve.X.Evaluate(progress),
                    curve.Y.Evaluate(progress),
                    curve.Z.Evaluate(progress)
                    );
                System.Diagnostics.Debug.Print("{0},{1},{2}", x.X, x.Y, x.Z);
                return x;
            });
        }
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }
}