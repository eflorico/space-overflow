using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public abstract class SortableQuestionSource : QuestionSource
    {
        public QuestionSort Sort { get; set; }
        public Order Order { get; set; }

        protected override void BeginFetchQuestions(int offset, int? count, Action<IEnumerable<Question>> success, Action<Exception> error) {
            try {
                if (!count.HasValue) count = 100;
                var page = offset / 100;

                do {
                    var request = this.BuildRequest();
                    request.Sort = this.Sort;
                    request.Order = this.Order;
                    request.Page = ++page;
                    request.PageSize = Math.Min(100, count.Value - ((page - 1) * 100 - offset));

                    this.PendingRequests.Add(request);

                    request.Begin(new Action<APIDataResponse<Question>>(response => {
                        this.Total = response.Total;
                        success(response.Items);
                    }), error);
                } while(page * 100 < offset + count.Value);
            }
            catch (Exception ex) {
                error(ex);
            }
        }

        protected int? Total;

        public override bool CanFetchMoreQuestions {
            get { return !this.Total.HasValue || this.Total.Value > this.AllQuestions.Count; }
        }

        protected abstract APISortedDataRequest<Question, QuestionSort> BuildRequest();
    }
}
