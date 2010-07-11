using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow.UI
{
    /// <summary>
    /// A graphical UI element.
    /// </summary>
    public abstract class UIElement
    {
        public UIElement() {
            this.Children = new List<UIElement>();
            this.Backgrounds = new List<Background>();
            this.IsVisible = true;
            this.Size = new Vector2(-1, -1);
        }

        private UIElement _parent;
        private SpriteFont _font;
        private Color? _foreground;
        private TextShadow _textShadow;
        private UIManager _manager;

        /// <summary>
        /// Gets or sets the visual parent of the element. It is not necessary to call AddChild/RemoveChild on the parent when the parent is set using this property.
        /// </summary>
        public UIElement Parent {
            get {
                return this._parent;
            }
            set {
                var oldParent = this._parent;
                this._parent = value;
                if (oldParent != null && oldParent.Children.Contains(this)) oldParent.RemoveChild(this); //Remove from old parent
                if (value != null) {
                    if (!value.Children.Contains(this)) value.AddChild(this); //Add to new parent
                    this.Manager = value.Manager; //Copy manager from parent
                }
            }
        }

        /// <summary>
        /// The UIManager that manages the focus for this element. Is automatically passed on to children when set.
        /// </summary>
        public UIManager Manager {
            get { return this._manager; }
            set {
                this.Children.ForEach(child => child.Manager = value);
                this._manager = value;
            }
        }

        /// <summary>
        /// List of visual children. Use Add-/Insert-/RemoveChild to modify!!
        /// </summary>
        public List<UIElement> Children { get; private set; }

        /// <summary>
        /// Adds a visual child.
        /// </summary>
        public virtual void AddChild(UIElement child) {
            if (child == null) throw new ArgumentNullException("child");
            else if (child == this) throw new ArgumentOutOfRangeException("child", "child cannot be this");

            this.Children.Add(child);

            if (child.Parent != this) child.Parent = this;
        }

        /// <summary>
        /// Inserts a visual child at the given index.
        /// </summary>
        public void InsertChild(int index, UIElement child) {
            if (child == null) throw new ArgumentNullException("child");
            else if (child == this) throw new ArgumentOutOfRangeException("child", "child cannot be this");

            this.Children.Insert(index, child);

            if (child.Parent != this) child.Parent = this;
        }

        /// <summary>
        /// Removes a visual child.
        /// </summary>
        /// <param name="child"></param>
        public virtual void RemoveChild(UIElement child) {
            this.Children.Remove(child);

            if (child.Parent == this) child.Parent = null;
        }

        /// <summary>
        /// The position of the element, in global coordinates.
        /// </summary>
        public Vector2 Position { get; set; }

        /// <summary>
        /// The desired size of the element, not including the padding. Set negative value for dimension to allow arbitrary size.
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// The bounds of the element, in global coordinates, not including the padding.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                var size = this.Measure();
                return new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)size.X, (int)size.Y);
            }
        }

        /// <summary>
        /// Space between outer element box and element content.
        /// </summary>
        public Thickness Padding { get; set; }

        /// <summary>
        /// The font used on this element. Value is inherited to visual children.
        /// </summary>
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

        /// <summary>
        /// The foreground color used on this element. Value is inherited to visual children.
        /// </summary>
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

        /// <summary>
        /// The text shadow that is applied to any text drawn by this element. Value is inherited to visual children.
        /// </summary>
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

        /// <summary>
        /// Determines if the element is drawn. Element will still be arranged if set to false!
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Corner radius that is applied to the background.
        /// </summary>
        public Texture2D CornerMask { get; set; }

        /// <summary>
        /// List of backgrounds. Drawn in specified order.
        /// </summary>
        public List<Background> Backgrounds { get; private set; }

        /// <summary>
        /// Positions the element's visual children.
        /// </summary>
        public virtual void Arrange() {
            foreach (var child in this.Children) {
                child.Position = this.Position;
                child.Arrange();
            }
        }

        /// <summary>
        /// Measures the element (not including padding). May depend on preceding Arrange call.
        /// </summary>
        public abstract Vector2 Measure();

        /// <summary>
        /// Draws the element to the specified SpriteBatch.
        /// </summary>
        /// <param name="target"></param>
        public void DrawTo(SpriteBatch target) {
            if (this.IsVisible) this.DrawOverride(target);
        }

        /// <summary>
        /// Virtual function to draw element.
        /// </summary>
        protected virtual void DrawOverride(SpriteBatch target) {
            this.DrawBackgrounds(target);
            this.DrawChildren(target);
        }

        /// <summary>
        /// Draws the element's backgrounds.
        /// </summary>
        /// <param name="target"></param>
        protected void DrawBackgrounds(SpriteBatch target) {
            if (this.Backgrounds.Count == 0) return;


            var fillRect = this.Bounds; //Target rectangle for background

            if (false && this.CornerMask != null) {
                target.End();

                var x = this.Manager.Device.GetRenderTarget(0);
                var tempRenderTarget = (RenderTarget2D)this.Manager.Device.GetRenderTarget(0);

                var newRenderTarget = new RenderTarget2D(this.Manager.Device, this.Manager.Device.PresentationParameters.BackBufferWidth, this.Manager.Device.PresentationParameters.BackBufferHeight, 1, this.Manager.Device.DisplayMode.Format, this.Manager.Device.PresentationParameters.MultiSampleType, this.Manager.Device.PresentationParameters.MultiSampleQuality);
                this.Manager.Device.SetRenderTarget(0, newRenderTarget);
                this.Manager.Device.Clear(Color.TransparentBlack);

                target.Begin();
                {
                    target.Draw(this.CornerMask, new Vector2(), Color.White);
                    target.Draw(this.CornerMask, new Vector2(fillRect.Width - this.CornerMask.Width, 0), Color.White, SpriteEffects.FlipHorizontally);
                    target.Draw(this.CornerMask, new Vector2(0, fillRect.Height - this.CornerMask.Height), Color.White, SpriteEffects.FlipVertically);
                    target.Draw(this.CornerMask, new Vector2(fillRect.Width - this.CornerMask.Width, fillRect.Height - this.CornerMask.Height), Color.White, SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally);

                    var white = new Texture2D(this.Manager.Device, 1, 1);
                        white.FillSolid(Color.White);
                        target.Draw(white, new Rectangle(this.CornerMask.Width, 0, fillRect.Width - this.CornerMask.Width * 2, fillRect.Height), Color.White);
                        target.Draw(white, new Rectangle(0, this.CornerMask.Height, fillRect.Width, fillRect.Height - this.CornerMask.Height * 2), Color.White);
                   

                } target.End();

                

                this.Manager.Device.SetRenderTarget(0, tempRenderTarget);
                var mask = newRenderTarget.GetTexture();
                newRenderTarget.Dispose();

                target.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                this.Manager.MaskEffect.Parameters["TextureMask"].SetValue(mask);
                this.Manager.MaskEffect.Begin();
                this.Manager.MaskEffect.CurrentTechnique.Passes[0].Begin();
            }

            foreach (var background in this.Backgrounds)
                if (background.Position == BackgroundPosition.Fill)
                    target.Draw(background.Texture, fillRect, Color.White);
                else {
                    var rect = this.Bounds;

                    //Determine target rectangle fot this background and make fillRect smaller so that following backgrounds don't overlap
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

            if (false && this.CornerMask != null) {
                this.Manager.MaskEffect.CurrentTechnique.Passes[0].End();
                this.Manager.MaskEffect.End();
                target.End();
                target.Begin();
            }

        }

        /// <summary>
        /// Calls DrawTo on the element's children.
        /// </summary>
        /// <param name="target"></param>
        protected void DrawChildren(SpriteBatch target) {
            foreach (var child in this.Children)
                child.DrawTo(target);
        }

        /// <summary>
        /// Processes any mouse input.
        /// </summary>
        /// <returns>Returns true if input could be applied on the element or one of its children.</returns>
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

        private bool clicked;

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

        
        public event MouseEventHandler Clicked;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
    }

    
}
