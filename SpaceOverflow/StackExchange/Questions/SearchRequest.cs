using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public class SearchRequest : QuestionsRequestBase
    {
        public SearchRequest(StackAPI api)
            : base(api) {
            this.Tagged = new List<string>();
            this.NotTagged = new List<string>();
        }

        public string InTitle { get; set; }
        public List<string> Tagged { get; private set; }
        public List<string> NotTagged { get; private set; }

        protected override string Route {
            get {
                return "search";
            }
        }

        protected override Dictionary<string, string> Parameters {
            get {
                var parameters = base.Parameters;
                if (this.InTitle != null) parameters.Add("intitle", this.InTitle);
                if (this.Tagged.Count > 0) parameters.Add("tagged", this.Tagged.Aggregate("", (sum, item) => sum + item + ";"));
                if (this.NotTagged.Count > 0) parameters.Add("nottagged", this.Tagged.Aggregate("", (sum, item) => sum + item + ";"));
                return parameters;
            }
        }
    }
}
