using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SpaceOverflow.Effects;

namespace SpaceOverflow
{
    public partial class App
    {
        Queue<QuestionChange> PendingChanges = new Queue<QuestionChange>();
        DateTime LastPoll, LastChange;
        TimeSpan ChangeInterval;

        /// <summary>
        /// Polls the server for updates on the questions currently shown as well as for new questions that should appear in the user's field of view.
        /// </summary>
        protected void Poll() {
            if (this.QuestionSource == null || this.Questions.Count == 0) return;

            //Find range of questions to keep track of
            var closest = this.Questions.First(qis => -this.View.Translation.Z - qis.Position.Z >= this.NearPlane);
            var farest = this.Questions.Last(qis => -this.View.Translation.Z - qis.Position.Z <= this.FarPlane);
            var offset = this.Questions.IndexOf(closest);
            var count = this.Questions.IndexOf(farest) - offset;

            this.QuestionSource.BeginReloadQuestions(offset, count, change => {
                //Enqueue change and set interval to pop off all changes until next poll
                this.PendingChanges.Enqueue(change);
                this.ChangeInterval = new TimeSpan((DateTime.Now - this.LastPoll + new TimeSpan(0, 1, 0)).Ticks / this.PendingChanges.Count);
            }, ex => {
                Debug.Print("Error while polling: " + ex.Message);
            });

            this.LastPoll = DateTime.Now;
        }

        /// <summary>
        /// Visualizes the next change in the queue.
        /// </summary>
        protected void PopNextChange() {
            if (this.PendingChanges.Count == 0) return;

            var change = this.PendingChanges.Dequeue();
            if (this.PendingChanges.Count > 0) this.ChangeInterval = new TimeSpan((DateTime.Now - this.LastPoll + new TimeSpan(0, 1, 0)).Ticks / this.PendingChanges.Count);

            Debug.Print("Popped change: " + change.Type.ToString() + " on " + (change.Question ?? change.OldQuestion).Title);

            if (change.Type == QuestionChangeType.Added) { //Question added: Plop!
                var qis = this.MapQuestion(change.Question);
                this.Questions.Add(qis);
                this.Questions.Sort((a, b) => Math.Sign(a.Position.Z - b.Position.Z)); //TODO: Insert efficiently

                //Enlarge from 0 to a scale of 0.7, then return to regular scale
                var finalScale = qis.Scale;
                qis.Animate("Scale", 0f, 0.7f, new TimeSpan(0, 0, 0, 0, 500), Interpolators.QuadraticOut, () =>
                    qis.Animate("Scale", qis.Scale, new TimeSpan(0, 0, 0, 0, 200), Interpolators.QuadraticInOut));

                this.Plop.Play();
            }
            else if (change.Type == QuestionChangeType.Changed) { //Changed: Remap
                var qis = this.Questions.Find(i => i.Question == change.OldQuestion);
                qis.Question = change.Question;
                qis.Animate("Position", this.QuestionMapper(qis.Question), new TimeSpan(0, 0, 1), Interpolators.QuadraticInOut);
            }
            else if (change.Type == QuestionChangeType.Removed) { //TODO: Doodle Jump-like animation + sound?
                var question = this.Questions.Find(qis => qis.Question == change.OldQuestion);
                this.Questions.Remove(question);
            }

            this.LastChange = DateTime.Now;
        }
    }
}
