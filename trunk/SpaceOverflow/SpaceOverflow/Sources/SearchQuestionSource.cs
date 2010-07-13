using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class SearchQuestionSource : QuestionSource
    {
        public string InTitle { get; set; }
        protected SearchRequest Request;

        public override void BeginFetchMoreQuestions(Action<int> success, Action<Exception> error) {
            if (!this.CanFetchMoreQuestions) return;

            this.Abort();

            try {
                this.Request = new SearchRequest(this.API) {
                    InTitle = this.InTitle,
                    Sort = this.Sort,
                    Order = this.Order,
                    Page = this.AllQuestions.Count / 90 + 1,
                    PageSize = 90
                };

                this.Request.Begin(new Action<APIDataResponse<Question>>(response => {
                    lock (this.AllQuestions) this.AllQuestions.AddRange(response.Items);
                    this.CanFetchMoreQuestions = response.Page * response.PageSize < response.Total;
                    success(response.Items.Count());
                }), error);
            }
            catch (Exception ex) {
                error(ex);
            }
        }

        public override void BeginReloadQuestions(int offset, int count, Action<IEnumerable<QuestionChange>> success, Action<Exception> error) {
            throw new NotImplementedException();
        }

        public override void Abort() {
            if (this.Request != null) this.Request.Abort();
        }

        public override bool IsRunning {
            get { return this.Request != null && this.Request.IsRunning; }
        }
    }
}
