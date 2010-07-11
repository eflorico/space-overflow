using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StackExchange
{
    public class TimelineRequest : APIPagedDataRequest<TimelineItem>
    {
        public TimelineRequest(StackAPI api)
            : base(api) { }

        public int UserID { get; set; }

        protected override string Route {
            get { return "users/" + this.UserID.ToString() + "/timeline"; }
        }

        protected override IEnumerable<TimelineItem> ProcessResponseItems(Newtonsoft.Json.Linq.JObject response) {
            return from i in response["user_timelines"].Children()
                   select new TimelineItem(this.API) {
                        TimelineType = (TimelineType)Enum.Parse(typeof(TimelineType), i["timeline_type"].Value<string>()),
                        PostID = i["post_id"].Value<int>(),
                        PostType = (PostType)Enum.Parse(typeof(PostType), i["post_type"].Value<string>()),
                        CreationDate = i["creation_date"].Value<int>().ToDateTime(),
                        Description = i["description"].Value<string>()
                   };
        }
    }
}
