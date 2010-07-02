using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public class QuestionsRequest : QuestionsRequestBase
    {
        public QuestionsRequest(StackAPI api)
            : base(api) { }

        protected override string Route {
            get { return "questions"; }
        }
    }
}
