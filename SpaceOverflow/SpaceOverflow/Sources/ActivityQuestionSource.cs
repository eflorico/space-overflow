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

        protected UsersRequest UsersRequest;
        protected QuestionsRequestBase QuestionsRequest;

        public override void BeginFetchMoreQuestions(Action<int> success, Action<Exception> error) {
            throw new NotImplementedException();
        }

        public override void BeginReloadQuestions(int offset, int count, Action<IEnumerable<QuestionChange>> success, Action<Exception> error) {
            throw new NotImplementedException();
        }

        public override void Abort() {
            throw new NotImplementedException();
        }

        public override bool IsRunning {
            get { throw new NotImplementedException(); }
        }
    }
}
