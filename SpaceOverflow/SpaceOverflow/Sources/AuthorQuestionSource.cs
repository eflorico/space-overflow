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

        public override void BeginFetchMoreQuestions(Action<int> success, Action<Exception> error) {
            try {
                if (!this.AuthorID.HasValue) {
                    this.UsersRequest = new UsersRequest(this.API) {
                        Filter = this.AuthorName,
                        PageSize = 100
                    };
                    this.UsersRequest.Begin(new Action<APIDataResponse<User>>(userResponse => {
                        this.UsersRequest = null;

                        var users = userResponse.Items.OrderBy(user => {
                            if (user.DisplayName.ToLower() == this.AuthorName.ToLower()) return 1f;
                            else if (user.DisplayName.ToLower().Contains(this.AuthorName.ToLower())) return 0.5f;
                            else return 0f;
                        });

                        if (users.Count() == 0) error(new Exception("User not found"));

                        this.AuthorID = users.First().ID;

                        this.BeginFetchMoreQuestions(success, error);
                    }), error);
                }
                else {
                    this.QuestionsRequest = new UsersQuestionsRequest(this.API) {
                        Sort = this.Sort,
                        Order = this.Order,
                        Page = this.AllQuestions.Count / 90,
                        PageSize = 90,
                        UserID = this.AuthorID.Value
                    };

                    this.QuestionsRequest.Begin(questionResponse => {
                        lock (this.AllQuestions) this.AllQuestions.AddRange(questionResponse.Items);
                        this.CanFetchMoreQuestions = questionResponse.Page * questionResponse.PageSize < questionResponse.Total;
                        ++this.QuestionsRequest.Page;
                        success(questionResponse.Items.Count());
                    }, error);
                }
            }
            catch (Exception ex) {
                error(ex);
            }
        }

        public override void BeginReloadQuestions(int offset, int count, Action<IEnumerable<QuestionChange>> success, Action<Exception> error) {
            throw new NotImplementedException();
        }

        public override void Abort() {
            throw new NotImplementedException();
        }

        public override bool IsRunning {
            get { throw new NotImplementedException(); }
        }

       
    }
}
