using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public partial class App
    {
        QuestionSource QuestionSource;
        UsersRequest UsersRequest;

        void AbortLoading() {
            if (this.QuestionSource != null) this.QuestionSource.Abort();
            if (this.UsersRequest != null) this.UsersRequest.Abort();
        }

        void BeginLoadQuestions(Action<IEnumerable<Question>> success, Action<Exception> error) {
            if (this.QuestionSource != null) this.QuestionSource.Abort();

            this.BeginBuildQuestionSource(source => {
                this.QuestionSource = source;
                this.QuestionSource.BeginFetchMoreQuestions(count => success(this.QuestionSource.AllQuestions), error);
            }, error);
        }

        void BeginBuildQuestionSource(Action<QuestionSource> success, Action<Exception> error) {
            if (this.SourceButton.SelectedItem == null) error(new Exception("No source selected!"));

            var api = this.SourceButtons[this.SourceButton.SelectedItem];
            var sort = this.ZOrderButtons[this.ZOrderButton.SelectedItem];

            if (this.RequestTypeButton.SelectedItem == this.BrowseButton)
                success(new BasicQuestionSource() {
                    API = api,
                    Sort = sort,
                    Order = Order.Descending
                });
            else {
                if (this.SearchBox.Text == "") error(new Exception("Search field is empty!"));
                else if (this.SearchPicker.SelectedItem == this.InQuestionsButton)
                    success(new SearchQuestionSource() {
                        API = api,
                        Sort = sort,
                        Order = Order.Descending,
                        InTitle = this.SearchBox.Text
                    });
                else {
                    this.UsersRequest = new UsersRequest(api) {
                        Filter = this.SearchBox.Text
                    };
                    this.UsersRequest.Begin(response => {
                        var users = response.Items.OrderByDescending(user => {
                            if (user.DisplayName.ToLower() == this.SearchBox.Text.ToLower()) return 1f;
                            else if (user.DisplayName.ToLower().Contains(this.SearchBox.Text.ToLower())) return 0.5f;
                            else return 0f;
                        });

                        if (users.Count() == 0) error(new Exception("User not found!"));

                        if (this.SearchPicker.SelectedItem == this.ByAuthorButton)
                            success(new AuthorQuestionSource() {
                                API = api,
                                Sort = sort,
                                Order = Order.Descending,
                                AuthorID = users.First().ID
                            });
                        else {
                            var uid = users.First().ID;
                            success(new ActivityQuestionSource() {
                                API = api,
                                UserID = uid
                            });
                        }
                    }, error);
                }
            }
        }
    }
}
