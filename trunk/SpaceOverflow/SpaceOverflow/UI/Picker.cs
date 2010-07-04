using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceOverflow.UI
{
    /// <summary>
    /// A UIElement that allows the user to pick one of its logical children.
    /// </summary>
    public abstract class Picker : Container
    {
        public Picker() : base() { }

        /// <summary>
        /// The currently selected logical child.
        /// </summary>
        private UIElement _selectedItem;
        public UIElement SelectedItem {
            get {
                return this._selectedItem;
            }
            set {
                if (value != null && !this.Items.Contains(value)) throw new Exception("The specified element is not an item of this element!");
                var old = this._selectedItem;
                this._selectedItem = value;
                this.OnSelectedItemChanged(old, value);
            }
        }

        protected virtual void OnSelectedItemChanged(UIElement oldSelectedItem, UIElement newSelectedItem) {
            if (this.SelectedItemChanged != null)
                this.SelectedItemChanged(this, new SelectedItemChangedEventArgs() {
                    OldSelectedChild = oldSelectedItem,
                    NewSelectedChild = newSelectedItem
                });
        }

        protected override void OnItemAdded(UIElement item) {
            base.OnItemAdded(item);

            //Make new item the selected one if no item is selected
            if (this.SelectedItem == null) this.SelectedItem = item;
        }

        protected override void OnItemRemoved(UIElement item) {
            base.OnItemRemoved(item);

            //Set selected item to null if item was selected
            if (this.SelectedItem == item) this.SelectedItem = null;
        }

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;
    }
}
