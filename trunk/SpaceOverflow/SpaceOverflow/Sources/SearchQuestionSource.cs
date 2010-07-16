using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class SearchQuestionSource : SortableQuestionSource
    {
        public string InTitle { get; set; }

        protected override APISortedDataRequest<Question, QuestionSort> BuildRequest() {
            return new SearchRequest(this.API) {
                InTitle = this.InTitle
            };
        }
    }
}
