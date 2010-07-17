using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class AuthorQuestionSource : SortableQuestionSource
    {
        public int AuthorID { get; set; }

        protected override APISortedDataRequest<Question, QuestionSort> BuildRequest() {
            return new UsersQuestionsRequest(this.API) {
                UserID = this.AuthorID
            };
        }
    }
}
