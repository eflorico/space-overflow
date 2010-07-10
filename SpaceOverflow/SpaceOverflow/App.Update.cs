using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System;
using SpaceOverflow.Effects;

namespace SpaceOverflow
{
    public partial class App
    {
        bool vectorRendering = false;
        Vector2 MouseDownPosition;
        Animation ZoomAnimation;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.R) && this.LastKeyboardState.IsKeyUp(Keys.R)) this.vectorRendering = !this.vectorRendering;

            if (this.State != AppState.BrowserOpened) {
                if (!this.UpdateGUI()) this.UpdateSpace(gameTime);

                Animator.Update(gameTime);
            }
            else
                this.UpdateBrowser();

            this.LastKeyboardState = Keyboard.GetState();
            this.LastMouseState = Mouse.GetState();

            System.Windows.Forms.Application.DoEvents();

            base.Update(gameTime);
        }

        protected bool UpdateGUI() {
            this.ToolBar.Position = new Vector2(0, this.Window.ClientBounds.Height - this.ToolBar.Measure().Y);
            this.ToolBar.Size = new Vector2(this.Window.ClientBounds.Width, -1);
            this.ToolBar.Arrange();
            return this.ToolBar.HandleMouse(Mouse.GetState(), this.LastMouseState);
        }

        protected void UpdateBrowser() {
            if (!this.Browser.Visible)
                this.State = AppState.Ready;
            else {
                this.Browser.Left = this.Window.ClientBounds.Left;
                this.Browser.Top = this.Window.ClientBounds.Top;
                this.Browser.Height = this.Window.ClientBounds.Height;
                this.Browser.Width = this.Window.ClientBounds.Width;
                this.Browser.BringToFront();
            }
        }

        protected void UpdateSpace(GameTime gameTime) {
            var mouseState = Mouse.GetState();
            var keyboardState = Keyboard.GetState();

            //Create ray at mouse position straight through 3D space
            var nearFlat = new Vector3(mouseState.X, mouseState.Y, 0);
            var farFlat = new Vector3(mouseState.X, mouseState.Y, -1);
            var nearDeep = this.GraphicsDevice.Viewport.Unproject(nearFlat, this.Projection, this.View, this.World);
            var farDeep = this.GraphicsDevice.Viewport.Unproject(farFlat, this.Projection, this.View, this.World);

            var direction = nearDeep - farDeep;
            direction.Normalize();

            var ray = new Ray(nearDeep, direction);

            //Zooming
            var zoom = 0f;

            if (mouseState.ScrollWheelValue != LastMouseState.ScrollWheelValue) //Zoom with mouse wheel
                zoom = (mouseState.ScrollWheelValue - LastMouseState.ScrollWheelValue) / 4;
            else if (keyboardState.IsKeyDown(Keys.Add) && this.LastKeyboardState.IsKeyUp(Keys.Add))
                zoom = 60;
            else if (keyboardState.IsKeyDown(Keys.Subtract) && this.LastKeyboardState.IsKeyUp(Keys.Subtract))
                zoom = -60;

            if (zoom != 0) {
                Vector3 to;

                if (this.ZoomAnimation != null) {
                    to = (Vector3)this.ZoomAnimation.To;
                    Animator.Animations.Remove(this.ZoomAnimation);
                }
                else to = this.View.Translation;

                to -= ray.Direction * zoom;

                this.ZoomAnimation = new Animation(this, "View.Translation", to, new TimeSpan(0, 0, 0, 0, 100));
                this.ZoomAnimation.Finished += new EventHandler((sender, e) => this.ZoomAnimation = null);
                Animator.Animations.Add(this.ZoomAnimation);

                //Apply translation
                //this.View.Translation -= ray.Direction * zoom;

                //Expand population if necessary
                if (this.CurrentRequest != null && !this.CurrentRequest.IsLoading && this.Questions.Count > 0 && -this.View.Translation.Z - 2000 < this.Questions.Min(q => q.Position.Z)) {
                    ++this.CurrentRequest.Page;
                    this.LoadAndExpand();
                }
            }

            //Mouse down...
            if (mouseState.LeftButton == ButtonState.Pressed && this.LastMouseState.LeftButton == ButtonState.Released && this.PanForce == Vector3.Zero)
                foreach (var question in this.Questions.OrderByDescending(q => q.Position.Z)) {
                    if (ray.Intersects(question.BoundingBox).HasValue) {
                        this.ClickedQuestion = question;
                        this.MouseDownPosition = mouseState.GetPosition();
                        break;
                    }
                }

            //...and up (finalize)
            if (mouseState.LeftButton == ButtonState.Released && this.ClickedQuestion != null &&
                    (this.MouseDownPosition - mouseState.GetPosition()).Length() < 5f &&
                    ray.Intersects(this.ClickedQuestion.BoundingBox).HasValue) {
                this.Browser.WebBrowser.Navigate(this.ClickedQuestion.Question.TimelineUri);
                this.Browser.Show();
                this.Browser.BringToFront();
                this.State = AppState.BrowserOpened;
            }

            //Or drop it?
            if (mouseState.LeftButton == ButtonState.Released)
                this.ClickedQuestion = null;

            //Pan by dragging
            if (mouseState.LeftButton == ButtonState.Pressed) {
                var currentMousePos = new Vector3(mouseState.X, -mouseState.Y, 0);

                if (this.LastMouseState.LeftButton == ButtonState.Pressed) {
                    var move = currentMousePos - new Vector3(this.LastMouseState.X, -this.LastMouseState.Y, 0);
                    if (move.Length() > 0) this.PanForce = move;
                    this.View.Translation += move;
                }
                else
                    this.PanForce = Vector3.Zero;
            }

            //Pan force
            var len = this.PanForce.Length();

            if (this.LastMouseState.LeftButton == ButtonState.Released && len > 0) {
                this.View.Translation += this.PanForce;
                if (len > 20)
                    this.PanForce *= 20 / len;
                if (len > 1)
                    this.PanForce *= 0.9f; //TODO: Make proportional to elapsed game time
                else
                    this.PanForce = Vector3.Zero;
            }
        }

        protected void Implode() {
            foreach (var qis in this.Questions)
                Animator.Animations.Add(new Animation(qis, "Position", new Vector3(0, 0, this.View.Translation.Z - this.NearPlane - this.FarPlane),
                    new TimeSpan(0, 0, 0, 0, 800), Interpolators.CubicIn));
        }

        protected void Explode() {
            this.ResetView();

            foreach (var qis in this.Questions)
                Animator.Animations.Add(new Animation(qis, "Position", new Vector3(0, 0, this.View.Translation.Z - this.NearPlane - this.FarPlane),
                    qis.Position,
                    new TimeSpan(0, 0, 0, 0, 800), Interpolators.CubicOut));
        }
    }
}