using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    /// <summary>
    /// Compares two StackAPI objects by their Endpoint URI.
    /// </summary>
    public class StackAPIComparer : IEqualityComparer<StackAPI>
    {
        private StackAPIComparer() { }

        private static StackAPIComparer _instance;

        public static StackAPIComparer Instance {
            get {
                if (_instance == null) _instance = new StackAPIComparer();
                return _instance;
            }
        }

        #region IEqualityComparer<StackAPI> Member

        public bool Equals(StackAPI x, StackAPI y) {
            if (x == null && y == null) return true;
            else if (x == null || y == null) return false;
            else return x.Endpoint.Equals(y.Endpoint);
        }

        public int GetHashCode(StackAPI obj) {
            return obj.Endpoint.GetHashCode();
        }

        #endregion
    }
}
