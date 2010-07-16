using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public class BasicQuestionSource : QuestionSource
    {
        protected override void BeginFetchQuestions(int offset, int? count, Action<IEnumerable<Question>> success, Action<Exception> error) {
            try {
                var request = new QuestionsRequest(this.API) {
                    Sort = this.Sort,
                    Order = this.Order,
                    Page = offset / 100 + 1,
                    PageSize = count.HasValue ? count.Value : 100
                };
                this.PendingRequests.Add(request);
                request.Begin(new Action<APIDataResponse<Question>>(response => {
                    this.Total = response.Total;
                    success(response.Items);
                }), error);
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
