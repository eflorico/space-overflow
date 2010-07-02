using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceOverflow
{
    public struct Thickness
    {
        public Thickness(int overall) : this(overall, overall, overall, overall) { }

        public Thickness(int topBottom, int leftRight) : this(topBottom, leftRight, topBottom, leftRight) { }

        public Thickness(int top, int right, int bottom, int left)
        {
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            this.Left = left;
        }

        public int Top, Right, Bottom, Left;

        public static Rectangle operator +(Rectangle rect, Thickness thickness) 
        {
            rect.X -= thickness.Left;
            rect.Y -= thickness.Top;
            rect.Width += thickness.Left + thickness.Right;
            rect.Height += thickness.Top + thickness.Bottom;

            return rect;
        }

        public static Rectangle operator -(Rectangle rect, Thickness thickness) 
        {
            rect.X += thickness.Left;
            rect.Y += thickness.Top;
            rect.Width -= thickness.Left + thickness.Right;
            rect.Height -= thickness.Top + thickness.Bottom;

            return rect;
        }

        public static Vector2 operator +(Vector2 size, Thickness thickness) {
            size.X += thickness.Left + thickness.Right;
            size.Y += thickness.Top + thickness.Bottom;

            return size;
        }

        public static Vector2 operator -(Vector2 size, Thickness thickness) {
            size.X -= thickness.Left + thickness.Right;
            size.Y -= thickness.Top + thickness.Bottom;

            return size;
        }
    }
}
