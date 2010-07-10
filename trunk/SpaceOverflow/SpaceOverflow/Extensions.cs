using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow
{
    public static class Extensions
    {
        public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Color color, SpriteEffects effects) {
            spriteBatch.Draw(texture, position, null, color, 0f, new Vector2(), 1f, effects, 0);
        }

        public static void DrawTiled(this SpriteBatch spriteBatch, Texture2D texture, Rectangle target, Color color) {
            spriteBatch.DrawTiled(texture, target, color, SpriteEffects.None);
        }

        public static void DrawTiled(this SpriteBatch spriteBatch, Texture2D texture, Rectangle target, Color color, SpriteEffects effects) {
            //spriteBatch.DrawTiled(texture, target, new Rectangle(0, 0, texture.Width, texture.Height), color, effects);
            spriteBatch.Draw(texture, target, null, color, 0f, new Vector2(), effects, 0);

            return; //TODO: Fix or drop
            int targetX = target.X, targetY = target.Y,
               sourceX = 0, sourceY = 0;

            do {
                do {
                    var width = Math.Min(target.Right - targetX, texture.Width - sourceX);
                    var height = Math.Min(target.Bottom - targetY, texture.Height - sourceY);

                    spriteBatch.Draw(texture,
                        new Rectangle(targetX, targetY, width, height),
                        new Rectangle(sourceX, sourceY, width, height),
                        color,
                        0f,
                        new Vector2(),
                        effects,
                        0);

                    targetX += width;
                    targetY += height;

                    sourceX += width;
                    sourceY += height;

                    if (sourceX == texture.Width) sourceX = 0;
                    if (sourceY == texture.Height) sourceY = 0;
                }
                while (targetY < target.Bottom);
            }
            while (targetX < target.Right);
        }

        public static void DrawTiled(this SpriteBatch spriteBatch, Texture2D texture, Rectangle target, Rectangle source, Color color, SpriteEffects effects) {
            int targetX = target.X, targetY = target.Y, 
                sourceX = source.X % texture.Width, sourceY = source.Y % texture.Height;

            do {
                do {
                    var width = Math.Min(target.Right - targetX, source.Right - sourceX);
                    var height = Math.Min(target.Bottom - targetY, source.Bottom - sourceY);

                    spriteBatch.Draw(texture,
                        new Rectangle(targetX, targetY, width, height),
                        new Rectangle(sourceX, sourceY, width, height),
                        color,
                        0f,
                        new Vector2(),
                        effects,
                        0);

                    targetX += width;
                    targetY += height;

                    sourceX += width;
                    sourceY += height;

                    if (sourceX == source.Right) sourceX = source.X;
                    if (sourceY == source.Bottom) sourceY = source.Y;
                }
                while (targetY < target.Bottom);
            }
            while (targetX < target.Right);
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

        public static float ExponentialInterpolate(float value1, float value2, float amount) {
            value2 -= value1;
            amount -= value1;

            var final = (float)Math.Sqrt(value2);
            var pos = amount * final;

            return pos * pos;
        }

        public static Vector2 GetPosition(this MouseState mouseState) {
            return new Vector2(mouseState.X, mouseState.Y);
        }
    }

    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }
}