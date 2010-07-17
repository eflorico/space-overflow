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
        Func<Question, Vector3> PositionMapper;
        Func<Question, float> ZMapper, RMapper, ThetaMapper;
        Func<Vector3, float> OpacityMapper;
        int ZCounter;

        /// <summary>
        /// Recreates the mappers for r and theta axes.
        /// </summary>
        protected void ReMapRAndTheta() {
            var questions = this.Questions.Select(qis => qis.Question);
            this.CreateRMapper(questions);
            this.CreateThetaMapper(questions);
            this.ZCounter = 0;
            this.Questions.ForEach(qis => {
                qis.Animate("Position", this.PositionMapper(qis.Question), new TimeSpan(0, 0, 1), Interpolators.QuadraticInOut);
            });
        }

        /// <summary>
        /// Completely maps a single question to a QuestionInSpace using the existing mappers.
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        protected QuestionInSpace MapQuestion(Question question) {
            var position = this.PositionMapper(question);
            return new QuestionInSpace() {
                Question = question,
                Position = position,
                TextSize = this.QuestionFont.MeasureString(question.Title),
                Scale = 0.3f,
                //Text = this.VectorQuestionFont.Fill(question.Title) //TODO: Drop if using sprite fonts only
            };
        }

        /// <summary>
        /// Generates all mappers except for opacity.
        /// </summary>
        protected void CreateMappers(IEnumerable<Question> questions) {
            this.CreateRMapper(questions);
            this.CreateThetaMapper(questions);
            this.CreateZMapper(questions);
            
            this.PositionMapper = new Func<Question, Vector3>(q => {
                var z = this.ZMapper(q) * 3000 - 3000;
                var r = (1 - this.RMapper(q)) * 700;
                var theta = this.ThetaMapper(q);

                return new Vector3(r * (float)Math.Cos(theta), r * (float)Math.Sin(theta), z);
            });
        }

        /// <summary>
        /// Generate r mapper according to user selection.
        /// </summary>
        protected void CreateRMapper(IEnumerable<Question> questions) {
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
        }

        /// <summary>
        /// Generate theta mapper, locating similarly tagged question at the same place.
        /// </summary>
        /// <param name="questions"></param>
        protected void CreateThetaMapper(IEnumerable<Question> questions) {
            //Find most oftenly used tags
            var topTags = questions.SelectMany(q => q.Tags).GroupBy(tag => tag)
                .Select(group => new KeyValuePair<string, int>(group.Key, group.Count())).Take(1000)
                .OrderByDescending(pair => pair.Value).Select(pair => new {
                    Tag = pair.Key,
                    Items = pair.Value,
                    StartAngle = 0f,
                    AngleRange = 0f
                }).ToList();

            //Create dictionary with top tags
            var topTagCounts = new Dictionary<object, int>();
            foreach (var topTag in topTags) topTagCounts.Add(topTag, 0);

            //Allocate questions to tags and track usage of top tags
            var questionsToTags = questions.Select(q => {
                var theTopTag = (from tag in q.Tags join topTag in topTags on tag equals topTag.Tag select topTag).FirstOrDefault();
                if (theTopTag != null) ++topTagCounts[theTopTag];
                return new {
                    Question = q,
                    TopTag = theTopTag
                };
            });

            //Copy usage numbers from dictionary to top tag collection
            topTags = topTags.Select(topTag => new {
                Tag = topTag.Tag,
                Items = topTagCounts[topTag],
                StartAngle = 0f,
                AngleRange = 0f
            }).Where(topTag => topTag.Items > 0).ToList();

            var totalQuestionCount = questions.Count();

            //Determine angles of top tags
            var i = 0f;
            topTags = topTags.Select((tag, index) => {
                var newTag = new {
                    Tag = tag.Tag,
                    Items = tag.Items,
                    StartAngle = i,
                    AngleRange = (float)tag.Items / (float)totalQuestionCount * (float)Math.PI * 2f,
                };

                i += newTag.AngleRange;

                return newTag;
            }).ToList();

            this.ThetaMapper = new Func<Question, float>(q => {
                var possibleTopTags = from tag in q.Tags join topTag in topTags on tag equals topTag.Tag orderby topTag.Items descending select topTag;
                var theTopTag = possibleTopTags.FirstOrDefault();

                if (theTopTag != null) //Place question in top tag area and add random but deterministic component
                    return theTopTag.StartAngle + ((float)q.ID % 100f) / 100f * theTopTag.AngleRange;
                else //Only random component for questions that are not tagged with any of the top tags
                    return q.ID * q.OwnerID % 143268;
            });
        }

        /// <summary>
        /// Creates the z mapper according to user selection.
        /// </summary>
        protected void CreateZMapper(IEnumerable<Question> questions) {
            Func<Question, float> zCriterionSelector = null;
            float minZ, maxZ;
            this.ZCounter = 0;

            var sort = this.ZOrderButtons[this.ZOrderButton.SelectedItem];

            switch (sort) {
                case QuestionSort.Creation:
                    zCriterionSelector = new Func<Question, float>(q => (float)q.CreationDate.ToUnixTimestamp());
                    break;
                case QuestionSort.Votes:
                    zCriterionSelector = new Func<Question, float>(q => q.UpVoteCount - q.DownVoteCount);
                    break;
                case QuestionSort.Activity:
                    zCriterionSelector = new Func<Question, float>(q => q.LastActivityDate.ToUnixTimestamp());
                    break;
                default:
                    zCriterionSelector = new Func<Question, float>(q => this.ZCounter--);
                    break;
            }

            if (sort == QuestionSort.Featured || sort == QuestionSort.Hot) {
                maxZ = 0;
                minZ = -questions.Count();
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

        /// <summary>
        /// Creates the opacity mapper according to camera position.
        /// </summary>
        protected void CreateOpacityMapper() {
            this.OpacityMapper = new Func<Vector3, float>(position => {
                var z = position.Z;

                float farBegin = this.FarPlane, farEnd = this.FarFade, nearEnd = this.NearFade, nearBegin = this.NearPlane;
                var distance = -this.View.Translation.Z - z;

                if (distance >= farBegin || distance <= nearBegin) return 0; //Out of visible range
                else if (distance >= farEnd) return (-(distance - farEnd) + (farBegin - farEnd)) / (farBegin - farEnd); //Fade in at far plane
                else if (distance >= nearEnd) return 1; //Fully inside visible range
                else return (distance - nearBegin) / (nearEnd - nearBegin); //Fade in at near plane
            });
        }
    }
}
