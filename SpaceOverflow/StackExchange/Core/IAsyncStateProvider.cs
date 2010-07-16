using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public interface IAsyncStateProvider
    {
        bool IsRunning { get; }
        void Abort();
    }
}
