using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace StackExchange
{
    public abstract class QuestionsRequestBase : APISortedDataRequest<Question, QuestionSort>
    {
        public QuestionsRequestBase(StackAPI api)
            : base(api) { }

        protected override IEnumerable<Question> ProcessResponseItems(JObject response) {
            return from i in response["questions"].Children()
                   select new Question(this.API) {
                       ID = i["question_id"].Value<int>(),
                       Title = i["title"].Value<string>(),
                       TimelineUri = new Uri(this.API.BaseURI, i["question_timeline_url"].Value<string>()),
                       OwnerID = i["owner"] != null ? i["owner"]["user_id"].Value<int>() : 0,
                       OwnerReputation = i["owner"] != null ? i["owner"]["reputation"].Value<int>() : 0,
                       Tags = i["tags"].Children().Select(j => j.Value<string>()).ToList(),
                       CreationDate = i["creation_date"].Value<int>().ToDateTime(),
                       LastActivityDate = i["last_activity_date"].Value<int>().ToDateTime(),
                       AnswerCount = i["answer_count"].Value<int>(),
                       FavoriteCount = i["favorite_count"].Value<int>(),
                       UpVoteCount = i["up_vote_count"].Value<int>(),
                       DownVoteCount = i["down_vote_count"].Value<int>(),
                       ViewCount = i["view_count"].Value<int>()
                   };
        }
    }

    public enum QuestionSort
    {
        Creation,
        Featured,
        Hot,
        Votes,
        Activity
    }

    public enum Order
    {
        Descending,
        Ascending
    }
}