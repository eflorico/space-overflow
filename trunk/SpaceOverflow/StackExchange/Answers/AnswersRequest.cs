using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StackExchange
{
    public class AnswersRequest : APISortedDataRequest<Answer, QuestionSort>
    {
        public AnswersRequest(StackAPI api)
            : base(api) {
            this.IDs = new List<int>();
        }

        public List<int> IDs { get; private set; }

        protected override IEnumerable<Answer> ProcessResponseItems(JObject response) {
            return from i in response["answers"].Children()
                   select new Answer() {
                       ID = i["answer_id"].Value<int>(),
                       QuestionID = i["question_id"].Value<int>()
                   };
        }

        protected override string Route {
            get {
                if (this.IDs.Count > 0) return "answers/" + this.IDs.Aggregate("", (sum, item) => sum + item + ";").TrimEnd(';');
                else return "answers";
            }
        }
    }
}
