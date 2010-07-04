using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceOverflow.UI
{
    public delegate void MouseEventHandler(object sender, MouseEventArgs e);

    public class MouseEventArgs : EventArgs
    {
        public Vector2 Position { get; set; }
    }
}
