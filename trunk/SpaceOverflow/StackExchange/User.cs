using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public class User : APIObject
    {
        public User(StackAPI api)
            : base(api)
        { }

        public int ID { get; set; }
        public string DisplayName { get; set; }
    }
}
