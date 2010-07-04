using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceOverflow.UI
{
    public abstract class Focusable : UIElement
    {
        private bool _isFocused;
        public bool IsFocused {
            get {
                return this._isFocused;
            }
            set {
                if (this.Manager != null) {
                    this._isFocused = value;
                    if (value) {
                        if (this.Manager.Focus != this) this.Manager.Focus = this;
                        this.OnGotFocus();
                    }
                    else if (!value) {
                        if (this.Manager.Focus == this) this.Manager.Focus = null;
                        this.OnLostFocus();
                    }
                }
            }
        }

        public virtual void HandleCharacterInput(CharacterEventArgs evArgs) { }
        public virtual void HandleKeyStroke(KeyEventArgs evArgs) { }

        public event EventHandler GotFocus;
        public event EventHandler LostFocus;

        protected virtual void OnGotFocus() {
            if (this.GotFocus != null) this.GotFocus(this, EventArgs.Empty);
        }

        protected virtual void OnLostFocus() {
            if (this.LostFocus != null) this.LostFocus(this, EventArgs.Empty);
        }
    }
}
