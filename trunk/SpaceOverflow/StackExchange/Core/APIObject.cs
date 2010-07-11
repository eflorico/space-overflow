using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public abstract class APIObject
    {
        protected StackAPI API { get; private set; }

        public APIObject(StackAPI api)
        {
            this.API = api;
        }
    }
}
