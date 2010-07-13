using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow.UI
{
    /// <summary>
    /// A drop-down menu that allows the user to pick an item.
    /// </summary>
    class DropDownButton : Picker
    {
        public DropDownButton()
            : base() {
            this.DropDownMenu = new StackPanel() {
                Orientation = Orientation.Vertical
            };
            this.AddChild(this.DropDownMenu);

            this.Button = new Button();
            this.Button.Clicked += this.OnSelectedItemClicked;
            this.AddChild(this.Button);

            this.IsDropped = false;
        }

        protected override void OnItemAdded(UIElement item) {
            base.OnItemAdded(item);

            //Add item to drop down if it's not selected
            if (this.SelectedItem != item) this.DropDownMenu.AddChild(item);
            item.Clicked += this.ItemClicked;
        }

        protected override void OnItemRemoved(UIElement item) {
            base.OnItemRemoved(item);

            //Remove item from drop-down
            this.DropDownMenu.RemoveChild(item);
            item.Clicked -= this.ItemClicked;
        }

        protected override void OnSelectedItemChanged(UIElement oldSelectedItem, UIElement newSelectedItem) {
            base.OnSelectedItemChanged(oldSelectedItem, newSelectedItem);

            if (oldSelectedItem != null && this.Items.Contains(oldSelectedItem)) //Move previously selected item to drop-down
                this.DropDownMenu.InsertChild(this.Items.IndexOf(oldSelectedItem), oldSelectedItem);
            if (newSelectedItem != null) { //Drop new selected item from drop-down, set button's text to item's text
                this.DropDownMenu.RemoveChild(newSelectedItem);
                if (newSelectedItem is Button) this.Button.Text = (newSelectedItem as Button).Text;
            }
        }

        protected virtual void OnSelectedItemClicked(object sender, EventArgs e) {
            this.IsDropped = !this.IsDropped; //Toggle drop-down
        }

        private void ItemClicked(object sender, EventArgs e) {
            var el = (sender as UIElement);
            if (el != null && el != this.SelectedItem) {
                this.SelectedItem = (UIElement)sender; //Change selected item
                this.IsDropped = false;
            }
        }

        public bool IsDropped {
            get { return this.DropDownMenu.IsVisible; }
            set { this.OnDropping(); this.DropDownMenu.IsVisible = value; }
        }

        protected void OnDropping() {
            if (this.Dropping != null) this.Dropping(this, EventArgs.Empty);
        }

        //public Texture2D SelectedIndicator { get; set; }
        /// <summary>
        /// StackPanel used to display the drop-down menu.
        /// </summary>
        public StackPanel DropDownMenu { get; private set; }

        /// <summary>
        /// Button used to display the currently selected item.
        /// </summary>
        public Button Button { get; private set; }

        public event EventHandler Dropping;

        public override void Arrange() {
            //Temporarily add selected item to drop-down to measure it under same conditions
            //(Property inheritance doesn't work when item is cut off from visual element tree)
            if (this.SelectedItem != null) this.DropDownMenu.AddChild(this.SelectedItem); 

            //Find biggest item
            var itemSizes = this.Items.Select(item => item.Measure() - item.Padding);
            var width = itemSizes.Count() > 0 ? itemSizes.Max(sz => sz.X) : 0;
            var height = itemSizes.Count() > 0 ? itemSizes.Max(sz => sz.Y) : 0;

            if (this.SelectedItem != null) this.DropDownMenu.RemoveChild(this.SelectedItem);

            //Set size of all items to the one of the biggest
            foreach (var item in this.Items)
                item.Size = new Vector2(width, height);

            if (this.IsDropped) { //Draw drop-down menu
                this.DropDownMenu.Arrange(); //Arrange to measure
                var dropDownSize = this.DropDownMenu.Measure();

                this.DropDownMenu.Position = new Vector2(
                    this.Bounds.Center.X - (int)dropDownSize.X / 2,
                    this.Position.Y - 4 - (int)dropDownSize.Y
                );

                this.DropDownMenu.Arrange();
            }

            //Arrange Button representing selected item
            this.Button.Position = this.Position;
            this.Button.Size = new Vector2(width, this.Button.Size.Y);
            this.Button.Arrange();
        }

        public override Vector2 Measure() {
            return this.Button.Measure();
        }

        public override bool HandleMouse(MouseState mouseState, MouseState lastMouseState) {
            //Pass responsiblity to selected item if drop-down menu is not shown
            if (this.Button.HandleMouse(mouseState, lastMouseState) && !this.IsDropped) return true; 

            //Otherwise, pass it to drop-down menu
            if (this.IsDropped) return this.DropDownMenu.HandleMouse(mouseState, lastMouseState);
            else return false; //Or give it back to the parent
        }
    }
}
