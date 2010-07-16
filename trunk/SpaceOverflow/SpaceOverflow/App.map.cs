using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StackExchange;
using SpaceOverflow.Effects;
using Microsoft.Xna.Framework;

namespace SpaceOverflow
{
    public partial class App
    {
        protected void ReMap() {
            this.CreateRAndThetaMappers(this.Questions.Select(qis => qis.Question));
            this.Questions.ForEach(qis => {
                qis.Animate("Position", this.QuestionMapper(qis.Question), new TimeSpan(0, 0, 1), Interpolators.QuadraticInOut);
            });
        }

        protected QuestionInSpace MapQuestion(Question question) {
            return new QuestionInSpace() {
                Question = question,
                Position = this.QuestionMapper(question),
                TextSize = this.QuestionFont.MeasureString(question.Title),
                Scale = 0.3f,
                Text = this.VectorQuestionFont.Fill(question.Title) //TODO: Drop if using sprite fonts only
            };
        }

        protected void CreateMappers(IEnumerable<Question> questions) {
            this.CreateRAndThetaMappers(questions);
            this.CreateZMapper(questions);

            this.QuestionMapper = new Func<Question, Vector3>(q => {
                var z = this.ZMapper(q) * 3000 - 3000;
                var r = (1 - this.RMapper(q)) * 700;
                var theta = this.ThetaMapper(q);

                return new Vector3(r * (float)Math.Cos(theta), r * (float)Math.Sin(theta), z);
            });
        }

        protected void CreateRAndThetaMappers(IEnumerable<Question> questions) {
            Func<Question, float> rCriterionSelector = null;
            float minR, maxR;

            if (this.ROrderButton.SelectedItem == this.RActiveButton) rCriterionSelector = new Func<Question, float>(q => q.LastActivityDate.Ticks);
            else if (this.ROrderButton.SelectedItem == this.ROwnerReputationButton) rCriterionSelector = new Func<Question, float>(q => q.OwnerReputation);
            else if (this.ROrderButton.SelectedItem == this.RVotesButton) rCriterionSelector = new Func<Question, float>(q => q.UpVoteCount - q.DownVoteCount);

            minR = questions.Min(rCriterionSelector);
            maxR = questions.Max(rCriterionSelector);

            this.RMapper = new Func<Question, float>(q => {
                if (maxR - minR == 0) return 1;

                var relativeValue = (float)(rCriterionSelector(q) - minR);
                var range = (float)(maxR - minR);
                var ret = relativeValue / range;
                return ret;
            });

            this.ThetaMapper = new Func<Question, float>(q =>
                q.ID * q.OwnerID % 143268);
        }

        protected void CreateZMapper(IEnumerable<Question> questions) {
            Func<Question, float> zCriterionSelector = null;
            float minZ, maxZ;
            int counter = 0;

            var sort = this.ZOrderButtons[this.ZOrderButton.SelectedItem];

            switch (sort) {
                case QuestionSort.Creation:
                    zCriterionSelector = new Func<Question, float>(q => (float)q.CreationDate.ToUnixTimestamp());
                    break;
                case QuestionSort.Votes:
                    zCriterionSelector = new Func<Question, float>(q => q.UpVoteCount - q.DownVoteCount);
                    break;
                case QuestionSort.Activity:
                    zCriterionSelector = new Func<Question, float>(q => q.LastActivityDate.Ticks);
                    break;
                default:
                    zCriterionSelector = new Func<Question, float>(q => counter++);
                    break;
            }

            if (sort == QuestionSort.Featured || sort == QuestionSort.Hot) {
                minZ = 0;
                maxZ = questions.Count();
            }
            else {
                minZ = questions.Min(zCriterionSelector);
                maxZ = questions.Max(zCriterionSelector);
            }


            this.ZMapper = new Func<Question, float>(q => {
                if (maxZ - minZ == 0) return 1;

                var relativeValue = (float)(zCriterionSelector(q) - minZ);
                var range = (float)(maxZ - minZ);
                var ret = relativeValue / range;
                return ret;
            });
        } 
    }
}
