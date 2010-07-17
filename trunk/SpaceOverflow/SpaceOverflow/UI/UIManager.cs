using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceOverflow.UI
{
    public class UIManager
    {
        public UIManager(UIElement root, GraphicsDevice device) {
            this.UIRoot = root;
            this.Device = device;

            root.Manager = this;

            EventInput.CharEntered += new CharEnteredHandler((sender, e) => {
                if (this.Focus != null) this.Focus.HandleCharacterInput(e);
            });

            EventInput.KeyDown += new KeyEventHandler((sender, e) => {
                if (this.Focus != null) this.Focus.HandleKeyStroke(e);
            });
        }

        public UIElement UIRoot { get; private set; }
        public GraphicsDevice Device { get; private set; }

        private Focusable _focus;
        public Focusable Focus {
            get {
                return this._focus;
            }
            set {
                if (this._focus != value) {
                    var old = this._focus;
                    this._focus = value;

                    if (old != null && old.IsFocused) old.IsFocused = false;
                    if (value != null && !value.IsFocused) value.IsFocused = true;
                }
            }
        }
    }
}
