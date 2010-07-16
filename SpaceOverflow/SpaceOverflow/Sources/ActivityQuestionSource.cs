using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class ActivityQuestionSource : QuestionSource
    {
        public string AuthorName { get; set; }
        protected int? AuthorID;

        public override bool CanFetchMoreQuestions {
            get { throw new NotImplementedException(); }
        }

        protected override void BeginFetchQuestions(int offset, int? count, Action<IEnumerable<Question>> success, Action<Exception> error) {
            throw new NotImplementedException();
        }
    }
}
