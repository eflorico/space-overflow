using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public interface IAsyncOperationProvider<TReturn> : IAsyncStateProvider
    {
        void Begin(Action<TReturn> success, Action<Exception> error);
    }
}
