using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public interface IAsyncOperationProvider<TReturn>
    {
        void Begin(Action<TReturn> success, Action<Exception> error);
        bool IsRunning { get; }
        void Abort();
    }
}
