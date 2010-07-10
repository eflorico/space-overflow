using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow.Sources
{
    public class AuthorQuestionSource : QuestionSource
    {
        public AuthorQuestionSource(string authorName, StackAPI api)
            : base() {
            this.API = api;
            this.AuthorName = authorName;
        }

        protected StackAPI API;
        protected string AuthorName;
        protected int? AuthorID;

        protected UsersRequest UsersRequest;
        protected QuestionsRequestBase QuestionsRequest;

        public override void BeginFetchMoreQuestions(Action<int> success, Action<Exception> error) {
            if (!this.AuthorID.HasValue) {
                this.UsersRequest = new UsersRequest(this.API) {
                    Filter = this.AuthorName,
                    PageSize = 100
                };
                this.UsersRequest.BeginGetResponse(new Action<DataResponse<User>>(userResponse => {
                    this.UsersRequest = null;

                    var users = userResponse.Items.OrderBy(user => {
                        if (user.DisplayName.ToLower() == this.AuthorName.ToLower()) return 1f;
                        else if (user.DisplayName.ToLower().Contains(this.AuthorName.ToLower())) return 0.5f;
                        else return 0f;
                    });

                    if (users.Count() == 0) error(new Exception("User not found"));

                    this.QuestionsRequest = new UsersQuestionsRequest(this.API) {
                    };



                }));
            }
        }

        public override void Abort() {
            throw new NotImplementedException();
        }

        public override bool IsLoading {
            get { throw new NotImplementedException(); }
        }
    }
}
