using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow.Sources
{
    public class SimpleQuestionSource : QuestionSource
    {
        public SimpleQuestionSource(APIPagedDataRequest<Question> baseRequest)
            : base() {
            this.BaseRequest = baseRequest;
        }

        protected APIPagedDataRequest<Question> BaseRequest;

        public override void BeginFetchMoreQuestions(Action<int> success, Action<Exception> error) {
            if (!this.CanFetchMoreQuestions) return;

            this.Abort();

            try {
                this.BaseRequest.BeginGetResponse(new Action<DataResponse<Question>>(response => {
                    lock (this.AllQuestions) this.AllQuestions.AddRange(response.Items);
                    this.CanFetchMoreQuestions = response.Page * response.PageSize < response.Total;
                    success(response.Items.Count());
                }));
            }
            catch (Exception ex) {
                error(ex);
            }
        }

        public override void Abort() {
            this.BaseRequest.Abort();
        }

        public override bool IsLoading {
            get { return this.BaseRequest.IsLoading; }
        }
    }
}
