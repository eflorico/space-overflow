using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow.UI
{
    public abstract class UIElement
    {
        public UIElement() {
            this.Children = new List<UIElement>();
            this.Backgrounds = new List<Background>();
            this.IsVisible = true;
        }

        private Vector2 _position, _size = new Vector2(-1, -1);
        private Thickness _padding;
        private UIElement _parent;
        private SpriteFont _font;
        private Color? _foreground;
        private TextShadow _textShadow;
        
        public UIElement Parent {
            get {
                return this._parent;
            }
            set {
                var oldParent = this._parent;
                this._parent = value;
                if (oldParent != null && oldParent.Children.Contains(this)) oldParent.RemoveChild(this);
                if (value != null) {
                    if (!value.Children.Contains(this)) value.AddChild(this);
                    this.Manager = value.Manager;
                    this.Children.ForEach(child => child.Manager = this.Manager);
                }
            }
        }

        public UIManager Manager { get; set; }

        public List<UIElement> Children { get; private set; }

        public virtual void AddChild(UIElement child) {
            if (child == null) throw new ArgumentNullException("child");
            else if (child == this) throw new ArgumentOutOfRangeException("child", "child cannot be this");

            this.Children.Add(child);

            if (child.Parent != this) child.Parent = this;
        }

        public void InsertChild(int index, UIElement child) {
            if (child == null) throw new ArgumentNullException("child");
            else if (child == this) throw new ArgumentOutOfRangeException("child", "child cannot be this");

            this.Children.Insert(index, child);

            if (child.Parent != this) child.Parent = this;
        }

        public virtual void RemoveChild(UIElement child) {
            this.Children.Remove(child);

            if (child.Parent == this) child.Parent = null;
        }

        public Vector2 Position {
            get {
                return this._position;
            }
            set {
                this._position = value;
            }
        }

        public Vector2 Size {
            get {
                return this._size;
            }
            set {
                this._size = value;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                var size = this.Measure();
                return new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)size.X, (int)size.Y);
            }
        }

        public Thickness Padding {
            get {
                return this._padding;
            }
            set {
                this._padding = value;
            }
        }

        public SpriteFont Font {
            get {
                if (this._font != null) return this._font;
                else if (this.Parent != null) return this.Parent.Font;
                else return null;
            }
            set {
                this._font = value;
            }
        }

        public Color? Foreground {
            get {
                if (this._foreground.HasValue) return this._foreground.Value;
                else if (this.Parent != null) return this.Parent.Foreground;
                else return null;
            }
            set {
                this._foreground = value;
            }
        }

        public TextShadow TextShadow {
            get {
                if (this._textShadow != null) return this._textShadow;
                else if (this.Parent != null) return this.Parent.TextShadow;
                else return null;
            }
            set {
                this._textShadow = value;
            }
        }

        public bool IsVisible { get; set; }
        public List<Background> Backgrounds { get; private set; }

        public virtual void Arrange() {
            foreach (var child in this.Children) {
                child.Position = this.Position;
                child.Arrange();
            }
        }

        public abstract Vector2 Measure();

        public void DrawTo(SpriteBatch target) {
            if (this.IsVisible) this.DrawOverride(target);
        }

        protected virtual void DrawOverride(SpriteBatch target) {
            this.DrawBackgrounds(target);
            this.DrawChildren(target);
        }

        protected void DrawBackgrounds(SpriteBatch target) {
            var fillRect = this.Bounds;

            foreach (var background in this.Backgrounds)
                if (background.Position == BackgroundPosition.Fill)
                    target.Draw(background.Texture, fillRect, Color.White);
                else {
                    var rect = this.Bounds;

                    if (background.Position == BackgroundPosition.Left) {
                        rect.Width = background.Texture.Width;
                        fillRect.X += background.Texture.Width;
                        fillRect.Width -= background.Texture.Width;
                    }
                    else if (background.Position == BackgroundPosition.Top) {
                        rect.Height = background.Texture.Height;
                        fillRect.Y += background.Texture.Height;
                        fillRect.Height -= background.Texture.Height;
                    }
                    else if (background.Position == BackgroundPosition.Right) {
                        rect.Width = background.Texture.Width;
                        rect.X = this.Bounds.Right - rect.Width;
                        fillRect.Width -= background.Texture.Width;
                    }
                    else if (background.Position == BackgroundPosition.Bottom) {
                        rect.Height = background.Texture.Height;
                        rect.Y = this.Bounds.Bottom - rect.Height;
                        fillRect.Height -= background.Texture.Height;
                    }
                    else if (background.Position == BackgroundPosition.Center) {
                        rect.Width = background.Texture.Width;
                        rect.X = this.Bounds.Center.X - rect.Width / 2;
                    }

                    target.Draw(background.Texture, rect, null, Color.White, 0f, new Vector2(), background.Effects, 0);
                }
        }

        protected void DrawChildren(SpriteBatch target) {
            foreach (var child in this.Children)
                child.DrawTo(target);
        }

        public virtual bool HandleMouse(MouseState mouseState, MouseState lastMouseState) {
            var mousePoint = new Point(mouseState.X, mouseState.Y);

            //Pass on to children
            for (var i = 0; i < this.Children.Count; ++i) {
                var child = this.Children[i];
                if (child.HandleMouse(mouseState, lastMouseState)) return true;
                if (this.Children.Count <= i || this.Children[i] != child) --i;
            }

            //Hit test
            if (this.Bounds.Contains(mousePoint)) {
                //Detect click
                if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
                    this.OnMouseDown(new Vector2(mouseState.X, mouseState.Y) - this.Position);
                else if (mouseState.LeftButton == ButtonState.Released && lastMouseState.LeftButton == ButtonState.Pressed)
                    this.OnMouseUp(new Vector2(mouseState.X, mouseState.Y) - this.Position);

                return this.Children.Count == 0;
            }
            else return false;
        }

        protected virtual void OnClicked(Vector2 position) {
            if (this.Clicked != null) this.Clicked(this, new MouseEventArgs() { Position = position });

            if (this.Manager != null && this is Focusable) this.Manager.Focus = (Focusable)this;
            else if (this.Manager != null) this.Manager.Focus = null;
        }

        protected virtual void OnMouseDown(Vector2 position) {
            if (this.MouseDown != null) this.MouseDown(this, new MouseEventArgs() { Position = position });
            this.clicked = true;
        }

        protected virtual void OnMouseUp(Vector2 position) {
            if (this.MouseUp != null) this.MouseUp(this, new MouseEventArgs() { Position = position });

            if (this.clicked) {
                this.OnClicked(position);
                this.clicked = false;
            }
        }

        private bool clicked;
        public event MouseEventHandler Clicked;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
    }

    
}
