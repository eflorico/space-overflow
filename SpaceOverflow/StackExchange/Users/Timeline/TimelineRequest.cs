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
                        TimelineType = i["timeline_type"].Value<string>().ParseToEnum<TimelineType>(),
                        PostID = i.HasProperty("post_id") ? i["post_id"].Value<int?>() : null,
                        PostType = i.HasProperty("post_type") ? i["post_type"].Value<string>().ParseToEnum<PostType>() : (PostType?)null,
                        CreationDate = i.HasProperty("creation_date") ? i["creation_date"].Value<int>().ToDateTime() : (DateTime?)null,
                        Description = i["description"].Value<string>()
                   };
        }
    }
}
