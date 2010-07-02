using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StackExchange
{
    public class UsersRequest : APIDataRequest<UserSort, User>
    {
        public UsersRequest(StackAPI api)
            : base(api) { }

        public string Filter { get; set; }

        protected override string Route {
            get { return "users"; }
        }

        protected override Dictionary<string, string> Parameters {
            get {
                var parameters = base.Parameters;
                if (this.Filter != null) parameters.Add("filter", this.Filter);
                return parameters;
            }
        }

        protected override IEnumerable<User> ProcessResponseItems(JObject response) {
            return from i in response["users"].Children()
                   select new User(this.API) {
                       ID = i["user_id"].Value<int>(),
                       DisplayName = i["display_name"].Value<string>()
                   };
        }
    }

    public enum UserSort
    {
        Reputation,
        Creation,
        Name
    }
}
