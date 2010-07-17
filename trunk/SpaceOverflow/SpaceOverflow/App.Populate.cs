using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StackExchange;
using System.Threading;
using System.Diagnostics;
using SpaceOverflow.Effects;

namespace SpaceOverflow
{
    partial class App
    {
        List<QuestionInSpace> Questions;
        
        /// <summary>
        /// "Implodes" all visible questions and reloads from the source
        /// </summary>
        protected void ReloadAndPopulate() {
            this.ProgressLabel.Text = "Loading...";
            this.ProgressIndicator.IsVisible = true;

            this.Implode();
            this.PendingChanges.Clear();

            this.BeginLoadQuestions(new Action<IEnumerable<Question>>(questions => {
                lock (this.Questions) this.Questions.Clear();
                this.Repopulate(questions);
                this.ResetView();
                this.Explode();
                if (this.Questions.Count == 0) this.ProgressLabel.Text = "No questions found";
                else this.ProgressLabel.Text = "Ready";
                this.ProgressIndicator.IsVisible = false;
            }), ex => {
                Debug.Print("Error while loading quetions:");
                Debug.Print(ex.ToString());

                this.ProgressLabel.Text = "Error";
                this.ProgressIndicator.IsVisible = false;
            });

            this.LastPoll = DateTime.Now;
        }

        /// <summary>
        /// Loads further questions from the source and adds them behind the already visible ones
        /// </summary>
        protected void LoadAndExpand() {
            this.ProgressLabel.Text = "Loading...";
            this.ProgressIndicator.IsVisible = true;

            this.QuestionSource.BeginFetchMoreQuestions(count => {
                this.ExpandPopulation(this.QuestionSource.AllQuestions.Skip(this.QuestionSource.AllQuestions.Count - count));
                this.ProgressLabel.Text = "Ready";
                this.ProgressIndicator.IsVisible = false;
            }, ex => {
                this.ProgressLabel.Text = "Error";
                this.ProgressIndicator.IsVisible = false;
            });
        }

        /// <summary>
        /// Cleans up the space and repopulates it with the specified questions.
        /// </summary>
        /// <param name="rawQuestions"></param>
        protected void Repopulate(IEnumerable<Question> rawQuestions) {
            if (rawQuestions.Count() == 0) return;

            this.CreateMappers(rawQuestions);

            var questionsInSpace = rawQuestions.Select(q => this.MapQuestion(q)).OrderBy(qis => qis.Position.Z).ToList();

            lock (this.Questions) this.Questions = questionsInSpace;
        }

        /// <summary>
        /// Adds the specified questions to the space using existing mappers.
        /// </summary>
        /// <param name="rawQuestions"></param>
        protected void ExpandPopulation(IEnumerable<Question> rawQuestions) {
            if (rawQuestions.Count() == 0) return;

            var mappedQuestions = rawQuestions.Select(q => this.MapQuestion(q));
            var questionsInSpace = this.Questions.Union(mappedQuestions).OrderBy(qis => qis.Position.Z).ToList();

            lock (this.Questions) this.Questions = questionsInSpace;        }
    }
}
