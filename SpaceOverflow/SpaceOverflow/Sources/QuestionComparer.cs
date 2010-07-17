using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class QuestionIDComparer : IEqualityComparer<Question>
    {
        private QuestionIDComparer() { }

        private static QuestionIDComparer _instance;

        public static QuestionIDComparer Instance {
            get {
                if (_instance == null) _instance = new QuestionIDComparer();
                return _instance;
            }
        }

        #region IEqualityComparer<Question> Member

        public bool Equals(Question x, Question y) {
            if (x == null && y == null) return true;
            else if (x == null || y == null) return false;
            else return x.ID == y.ID;
        }

        public int GetHashCode(Question obj) {
            return obj.ID;
        }

        #endregion
    }
}
