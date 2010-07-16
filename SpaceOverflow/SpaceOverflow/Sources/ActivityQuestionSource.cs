using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class ActivityQuestionSource : QuestionSource
    {
        public int UserID { get; set; }
        protected IEnumerable<Question> ReceivedQuestions;

        protected override void BeginFetchQuestions(int offset, int? count, Action<IEnumerable<Question>> success, Action<Exception> error) {
            this.ReceivedQuestions = new List<Question>();

            var activityRequest = new TimelineRequest(this.API) {
                Page = offset / 100 + 1,
                PageSize = count.HasValue ? count.Value : 100,
                UserID = this.UserID
            };
            this.PendingRequests.Add(activityRequest);

            activityRequest.Begin(timelineResponse => {
                {
                    var questionIds = (from item in timelineResponse.Items
                                       where item.PostType == PostType.Question
                                       select item.PostID.Value).Distinct();
                    
                    this.BeginFetchQuestionsByID(questionIds, success, error);
                }
                {
                    var answerIds = (from item in timelineResponse.Items
                                     where item.PostType == PostType.Answer
                                     select item.PostID.Value).Distinct();

                    for (var i = 0; i < answerIds.Count(); i += 30) {
                        var answersRequest = new AnswersRequest(this.API);
                        answersRequest.IDs.AddRange(answerIds.Skip(i).Take(30));
                        this.PendingRequests.Add(answersRequest);

                        answersRequest.Begin(answersResponse => 
                            this.BeginFetchQuestionsByID(answersResponse.Items.Select(item => item.QuestionID), success, error)
                        , error);
                    }
                }

                this.Total = timelineResponse.Total;
            }, error);
        }

        protected void BeginFetchQuestionsByID(IEnumerable<int> ids, Action<IEnumerable<Question>> success, Action<Exception> error) {
            for (var i = 0; i < ids.Count(); i += 30) {
                var questionsRequest = new QuestionsRequest(this.API);
                questionsRequest.IDs.AddRange(ids.Skip(i).Take(30));
                this.PendingRequests.Add(questionsRequest);

                questionsRequest.Begin(questionsResponse => {
                    this.ReceivedQuestions = this.ReceivedQuestions.Union(questionsResponse.Items, QuestionIDComparer.Instance).ToList();

                    if (!this.IsRunning) success(this.ReceivedQuestions);
                }, error);
            }
        }

        protected int? Total;

        public override bool CanFetchMoreQuestions {
            get { return !this.Total.HasValue || this.Total.Value < this.AllQuestions.Count; }
        }

        
    }
}
