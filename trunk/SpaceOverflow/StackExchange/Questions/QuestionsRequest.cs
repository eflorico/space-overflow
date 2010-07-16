using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public class QuestionsRequest : QuestionsRequestBase
    {
        public QuestionsRequest(StackAPI api)
            : base(api) {
            this.IDs = new List<int>();
        }

        public List<int> IDs { get; private set; }

        protected override string Route {
            get {
                if (this.IDs.Count > 0) return "questions/" + this.IDs.Aggregate("", (sum, item) => sum + item + ";").TrimEnd(';');
                else return "questions";
            }
        }
    }
}
