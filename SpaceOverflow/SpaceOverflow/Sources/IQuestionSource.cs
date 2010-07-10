using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow.Sources
{
    public abstract class QuestionSource
    {
        public QuestionSource() {
            this.AllQuestions = new List<Question>();
            this.CanFetchMoreQuestions = true;
        }

        public List<Question> AllQuestions { get; private set; }
        public bool CanFetchMoreQuestions { get; protected set; }
        


        public abstract void BeginFetchMoreQuestions(Action<int> success, Action<Exception> error);
        public abstract void Abort();
        public abstract bool IsLoading { get; }

    }
}
