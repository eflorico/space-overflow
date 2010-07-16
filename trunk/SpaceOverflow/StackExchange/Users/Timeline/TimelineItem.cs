using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public class TimelineItem : APIObject
    {
        public TimelineItem(StackAPI api)
            : base(api) { }

        public TimelineType TimelineType { get; set; }

        public int? PostID { get; set; }
        public PostType? PostType { get; set; }

        public string Description { get; set; }
        public DateTime? CreationDate { get; set; }
    }

    public enum TimelineType
    {
        AskOrAnswered,
        Comment,
        Badge,
        Revision,
        Accepted
    }

    public enum PostType
    {
        Question,
        Answer
    }
}
