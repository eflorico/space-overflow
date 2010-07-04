using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using System.Timers;

namespace SpaceOverflow.UI
{
    public class TextBox : Focusable
    {
        public TextBox() {
            this.PlaceholderOpacity = 0.7f;
            this.Text = "";

            this.CaretBlinkTimer = new Timer(GetCaretBlinkTime());
            this.CaretBlinkTimer.Elapsed += new ElapsedEventHandler((sender, e) => this.IsCaretVisible = !this.IsCaretVisible);
        }

        public Texture2D Indicator { get; set; }
        public Texture2D Caret { get; set; }
        public string Placeholder { get; set; }
        public float PlaceholderOpacity {get;set;}
        public int CaretPosition { get; set; }
        public string Text { get; set; }

        public event KeyEventHandler KeyPressed;

        [DllImport("user32.dll")]
        protected static extern uint GetCaretBlinkTime();
        protected bool IsCaretVisible;
        protected Timer CaretBlinkTimer;

        protected void ResetCaretBlink() {
            this.CaretBlinkTimer.Stop();
            this.IsCaretVisible = true;
            this.CaretBlinkTimer.Start();
        }

        protected override void OnGotFocus() {
            base.OnGotFocus();

            this.ResetCaretBlink();
        }

        protected override void OnLostFocus() {
            base.OnLostFocus();

            this.CaretBlinkTimer.Stop();
            this.IsCaretVisible = false;
        }

        protected override void DrawOverride(SpriteBatch target) {
            base.DrawBackgrounds(target);

            var position = this.Position + new Vector2(this.Padding.Left, this.Padding.Top);

            if (this.Foreground.HasValue) {
                if (this.Text == "" && !this.IsFocused) target.DrawString(this.Font, this.Placeholder, position, new Color(this.Foreground.Value, this.PlaceholderOpacity));
                else target.DrawString(this.Font, this.Text, position, this.Foreground.Value);
            }

            if (this.IsFocused && this.Caret != null && this.IsCaretVisible)
                target.Draw(this.Caret, new Rectangle(
                    (int)position.X + (int)this.FindCharPosition(this.CaretPosition),
                    (int)position.Y,
                    this.Caret.Width,
                    this.Font.LineSpacing), Color.White);
        }

        public override Vector2 Measure() {
            var size = this.Size;

            if (size.X < 0 || size.Y < 0) {
                var text = this.Text == "" && !this.IsFocused ? this.Placeholder : this.Text;
                var textSize = this.Font.MeasureString(text);

                if (size.X < 0) size.X = (float)Math.Ceiling(textSize.X);
                if (size.Y < 0) size.Y = (float)Math.Ceiling(textSize.Y);
            }

            size.X += this.Padding.Left + this.Padding.Right;
            size.Y += this.Padding.Top + this.Padding.Bottom;

            return size;
        }

        protected float FindCharPosition(int index) {
            if (index > 0) return this.Font.MeasureString(this.Text.Substring(0, index)).X;
            else return 0;
        }

        protected int FindCharIndex(float position) {
            var textWidth = this.Font.MeasureString(this.Text).X;
            if (position > textWidth) return this.Text.Length;

            //Estimate, assuming monospaced font
            var index = (int)Math.Round(position / textWidth * (float)this.Text.Length);

            while (true) {
                var posForIndex = this.FindCharPosition(index);
                var offset = Math.Abs(position - posForIndex);

                float? posForPrev = null, posForNext = null;

                if (index > 0) posForPrev = this.FindCharPosition(index - 1) + (posForIndex - posForPrev) / 2;
                if (index < this.Text.Length) {
                    posForNext = this.FindCharPosition(index + 1);
                    posForIndex += (posForNext.Value - posForIndex) / 2;
                }

                if (posForPrev.HasValue && position - posForPrev < offset) --index;
                else if (posForNext.HasValue && posForNext - position < offset) ++index;
                else return index;
            }
        }

        public override void HandleCharacterInput(CharacterEventArgs evArgs) {
            base.HandleCharacterInput(evArgs);

            if (this.Font.Characters.Contains(evArgs.Character))
                this.Text = this.Text.Insert(this.CaretPosition++, evArgs.Character.ToString());
        }

        public override void HandleKeyStroke(KeyEventArgs evArgs) {
            if (evArgs.KeyCode == Keys.Left && this.CaretPosition > 0) --this.CaretPosition;
            else if (evArgs.KeyCode == Keys.Right && this.CaretPosition < this.Text.Length) ++this.CaretPosition;
            else if (evArgs.KeyCode == Keys.Back && this.CaretPosition > 0) this.Text = this.Text.Remove(--this.CaretPosition, 1);
            else if (evArgs.KeyCode == Keys.Delete && this.CaretPosition < this.Text.Length) this.Text = this.Text.Remove(this.CaretPosition, 1);
            else if (evArgs.KeyCode == Keys.Home) this.CaretPosition = 0;
            else if (evArgs.KeyCode == Keys.End) this.CaretPosition = this.Text.Length;

            this.ResetCaretBlink();
            this.OnKeyPressed(evArgs);
        }

        protected virtual void OnKeyPressed(KeyEventArgs e) {
            if (this.KeyPressed != null) this.KeyPressed(this, e);
        }

        protected override void OnClicked() {
            base.OnClicked();

            this.Manager.Focus = this;
            this.CaretPosition = this.FindCharIndex(Mouse.GetState().X - this.Position.X - this.Padding.Left);
        }

    }
}
