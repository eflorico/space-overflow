using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class QuestionComparer : IEqualityComparer<Question>
    {
        #region IEqualityComparer<Question> Member

        public bool Equals(Question x, Question y) {
            return x.ID == y.ID;
        }

        public int GetHashCode(Question obj) {
            throw new NotImplementedException();
        }

        #endregion
    }
}
