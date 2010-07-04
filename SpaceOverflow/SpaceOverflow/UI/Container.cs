using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
namespace SpaceOverflow.UI
{
    public abstract class Container : UIElement
    {
        public Container() : base() {
            this.Items = new List<UIElement>();
        }

        public List<UIElement> Items { get; private set; }

        public void AddItem(UIElement item) {
            if (item == null) throw new ArgumentNullException("item");
            if (this.Items.Contains(item)) throw new ArgumentException("Item cannot be added twice!");
            
            this.Items.Add(item);
            this.OnItemAdded(item);
        }

        public void RemoveItem(UIElement item) {
            if (this.Items.Remove(item)) this.OnItemRemoved(item);
        }

        protected virtual void OnItemAdded(UIElement item) { }
        protected virtual void OnItemRemoved(UIElement item) { }
    }
}
