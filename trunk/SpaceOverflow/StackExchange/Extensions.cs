using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public static class Extensions
    {
        public static int ToUnixTimestamp(this DateTime date)
        {
            return (int)(date - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime ToDateTime(this int timestamp)
        {
            return new DateTime(1970, 1, 1) + new TimeSpan(0, 0, timestamp);
        }
    }
}
