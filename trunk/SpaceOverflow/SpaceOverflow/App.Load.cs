using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public partial class App
    {
        /// <summary>
        /// Source of infinite wisdom.
        /// </summary>
        QuestionSource QuestionSource;

        /// <summary>
        /// Used to fetch user id from name when searching for question by author.
        /// </summary>
        UsersRequest UsersRequest;

        /// <summary>
        /// Aborts all currently pending requests.
        /// </summary>
        protected void AbortLoading() {
            if (this.QuestionSource != null) this.QuestionSource.Abort();
            if (this.UsersRequest != null) this.UsersRequest.Abort();
        }

        /// <summary>
        /// Rebuilds the question source according to user settings and begins loading.
        /// </summary>
        protected void BeginLoadQuestions(Action<IEnumerable<Question>> success, Action<Exception> error) {
            this.AbortLoading();

            this.BeginBuildQuestionSource(source => {
                this.QuestionSource = source;
                this.QuestionSource.BeginFetchMoreQuestions(count => success(this.QuestionSource.AllQuestions), error);
            }, error);
        }

        /// <summary>
        /// Begins building a question source according to user settings. Asynchronous because user id needs to be fetched on username-based requests.
        /// </summary>
        protected void BeginBuildQuestionSource(Action<QuestionSource> success, Action<Exception> error) {
            if (this.SourceButton.SelectedItem == null) {
                error(new Exception("No source selected!"));
                return;
            }

            if (this.ZOrderButton.SelectedItem == null) {
                error(new Exception("No z-order selected!"));
                return;
            }

            var api = this.SourceButtons[this.SourceButton.SelectedItem];
            var sort = this.ZOrderButtons[this.ZOrderButton.SelectedItem];

            //Basic browse request
            if (this.RequestTypeButton.SelectedItem == this.BrowseButton)
                success(new BasicQuestionSource() {
                    API = api,
                    Sort = sort,
                    Order = Order.Descending
                });
            else { //All kinds of search requests
                if (this.SearchBox.Text == "") error(new Exception("Search field is empty!"));
                //Search in questions
                else if (this.SearchPicker.SelectedItem == this.InQuestionsButton)
                    success(new SearchQuestionSource() {
                        API = api,
                        Sort = sort,
                        Order = Order.Descending,
                        InTitle = this.SearchBox.Text
                    });
                else { //Search requests including a user name
                    this.UsersRequest = new UsersRequest(api) {
                        Filter = this.SearchBox.Text
                    };
                    this.UsersRequest.Begin(response => { //Search for user
                        //Find best match
                        var users = response.Items.OrderByDescending(user => {
                            if (user.DisplayName.ToLower() == this.SearchBox.Text.ToLower()) return 1f;
                            else if (user.DisplayName.ToLower().Contains(this.SearchBox.Text.ToLower())) return 0.5f;
                            else return 0f;
                        });

                        if (users.Count() == 0) error(new Exception("User not found!"));

                        //Find questions by author
                        if (this.SearchPicker.SelectedItem == this.ByAuthorButton)
                            success(new AuthorQuestionSource() {
                                API = api,
                                Sort = sort,
                                Order = Order.Descending,
                                AuthorID = users.First().ID
                            });
                        else { //Or by activity of a user
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
