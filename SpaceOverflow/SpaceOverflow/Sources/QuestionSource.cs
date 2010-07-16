using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;

namespace SpaceOverflow
{
    public abstract class QuestionSource : IAsyncStateProvider
    {
        public QuestionSource() {
            this.AllQuestions = new List<Question>();
        }

        public StackAPI API { get; set; }
        public QuestionSort Sort { get; set; }
        public Order Order { get; set; }

        public List<Question> AllQuestions { get; private set; }
        public abstract bool CanFetchMoreQuestions { get; }

        protected abstract void BeginFetchQuestions(int offset, int? count, Action<IEnumerable<Question>> success, Action<Exception> error);

        public void Abort() {
            while (this.PendingRequests.Count > 0) {
                this.PendingRequests[0].Abort();
                this.PendingRequests.RemoveAt(0);
            }
        }

        public bool IsRunning {
            get {
                return this.PendingRequests.FirstOrDefault(req => req.IsRunning) != null;
            }
        }

        protected List<IAsyncStateProvider> PendingRequests = new List<IAsyncStateProvider>();

        public void BeginFetchMoreQuestions(Action<int> success, Action<Exception> error) {
            this.Abort();

            this.BeginFetchQuestions(this.AllQuestions.Count, null, questions => {
                lock (this.AllQuestions) this.AllQuestions.AddRange(questions);
                success(questions.Count());
            }, error);
        }

        public void BeginReloadQuestions(int offset, int count, Action<QuestionChange> success, Action<Exception> error) {
            this.Abort();

            try {
                var result = new List<Question>(this.AllQuestions);
                var ids = this.AllQuestions.Skip(offset).Take(count).Select(q => q.ID);

                for (var i = 0; i < ids.Count(); i += 30) {
                    var request = new QuestionsRequest(this.API);
                    this.PendingRequests.Add(request);
                    request.IDs.AddRange(ids.Skip(i).Take(30));

                    request.Begin(response => {
                        lock (this.AllQuestions) {
                            foreach (var oldID in request.IDs) {
                                var oldQuestion = this.AllQuestions.Find(q => q.ID == oldID);
                                var newQuestion = response.Items.FirstOrDefault(q => q.ID == oldQuestion.ID);

                                if (newQuestion != null) {
                                    if (oldQuestion.DownVoteCount != newQuestion.DownVoteCount ||
                                        oldQuestion.UpVoteCount != newQuestion.UpVoteCount ||
                                        oldQuestion.FavoriteCount != newQuestion.FavoriteCount ||
                                        oldQuestion.ViewCount != newQuestion.ViewCount ||
                                        oldQuestion.LastActivityDate != newQuestion.LastActivityDate ||
                                        oldQuestion.OwnerReputation != newQuestion.OwnerReputation) {
                                        success(new QuestionChange(QuestionChangeType.Changed, newQuestion, oldQuestion));
                                        result[result.IndexOf(oldQuestion)] = newQuestion;
                                    }
                                }
                                else {
                                    success(new QuestionChange(QuestionChangeType.Removed, null, oldQuestion));
                                    result.Remove(oldQuestion);
                                }
                            }

                            this.AllQuestions.Clear();
                            this.AllQuestions.AddRange(result);
                        }
                    }, error);
                }

                this.BeginFetchQuestions(offset, count, questions => {
                    foreach (var question in questions.Where(q => !this.AllQuestions.Contains(q, new QuestionComparer())))
                        success(new QuestionChange(QuestionChangeType.Added, question, null));
                        
                }, error);
            }
            catch (Exception ex) {
                this.Abort();
                error(ex);
            }
        }
    }
}
