using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public class UsersQuestionsRequest : QuestionsRequestBase
    {
        public UsersQuestionsRequest(StackAPI api)
            : base(api) { }

        public int UserID { get; set; }

        protected override string Route {
            get {
                return "users/" + this.UserID.ToString() + "/questions";
            }
        }
    }
}
