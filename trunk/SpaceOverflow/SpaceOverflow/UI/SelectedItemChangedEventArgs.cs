using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceOverflow.UI
{
    public class SelectedItemChangedEventArgs : EventArgs
    {
        public UIElement NewSelectedChild { get; set; }
        public UIElement OldSelectedChild { get; set; }
    }
}
