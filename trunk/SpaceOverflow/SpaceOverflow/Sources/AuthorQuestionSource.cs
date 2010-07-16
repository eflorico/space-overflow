using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class AuthorQuestionSource : QuestionSource
    {
        public string AuthorName { get; set; }
        protected int? AuthorID;

        protected UsersRequest UsersRequest;
        protected QuestionsRequestBase QuestionsRequest;


        protected override void BeginFetchQuestions(int offset, int? count, Action<IEnumerable<Question>> success, Action<Exception> error) {
            try {
                if (!this.AuthorID.HasValue) {
                    var request = new UsersRequest(this.API) {
                        Filter = this.AuthorName,
                        PageSize = 100
                    };
                    this.PendingRequests.Add(request);
                    request.Begin(new Action<APIDataResponse<User>>(response => {
                        var users = response.Items.OrderBy(user => {
                            if (user.DisplayName.ToLower() == this.AuthorName.ToLower()) return 1f;
                            else if (user.DisplayName.ToLower().Contains(this.AuthorName.ToLower())) return 0.5f;
                            else return 0f;
                        });

                        if (users.Count() == 0) error(new Exception("User not found"));

                        this.AuthorID = users.First().ID;

                        this.BeginFetchQuestions(offset, count, success, error);
                    }), error);
                }
                else {
                    var request = new UsersQuestionsRequest(this.API) {
                        Sort = this.Sort,
                        Order = this.Order,
                        Page = offset / 100 + 1,
                        PageSize = count.HasValue ? count.Value : 100,
                        UserID = this.AuthorID.Value
                    };
                    this.PendingRequests.Add(request);
                    request.Begin(response => {
                        this.Total = response.Total;
                        success(response.Items);
                    }, error);
                }
            }
            catch (Exception ex) {
                error(ex);
            }
        }

        protected int? Total;

        public override bool CanFetchMoreQuestions {
            get { return !this.Total.HasValue || this.Total.Value < this.AllQuestions.Count; }
        }
    }
}
