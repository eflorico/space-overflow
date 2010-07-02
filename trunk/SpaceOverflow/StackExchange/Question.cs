using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StackExchange
{
    public class Question : APIObject
    {
        public Question(StackAPI api)
            : base(api)
        {
            this.Tags = new List<string>();
        }

        public int ID { get; set; }

        public string Title { get; set; }

        public Uri TimelineUri { get; set; }

        public int OwnerID { get; set; }

        public int OwnerReputation { get; set; }

        public List<string> Tags { get; set; }

        public DateTime CreationDate { get; set; }

        public int AnswerCount { get; set; }

        public int FavoriteCount { get; set; }

        public int UpVoteCount { get; set; }

        public int DownVoteCount { get; set; }

        public int ViewCount { get; set; }
    }
}
