using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class BasicQuestionSource : SortableQuestionSource
    {
        protected override APISortedDataRequest<Question, QuestionSort> BuildRequest() {
            return new QuestionsRequest(this.API);
        }
    }
}
