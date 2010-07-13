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
        Func<Question, Vector3> QuestionMapper;
        Func<Question, float> ZMapper, RMapper, ThetaMapper;
        QuestionSource QuestionSource;

        protected void ReloadAndPopulate() {
            this.ProgressLabel.Text = "Loading...";
            this.ProgressIndicator.IsVisible = true;

            this.Implode();

            this.BeginLoadQuestions(new Action<IEnumerable<Question>>(questions => {
                lock (this.Questions) this.Questions.Clear();
                this.Repopulate(questions);
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

        }

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

        protected QuestionSource BuildQuestionSource() {
            if (this.SourceButton.SelectedItem == null) return null;

            var api = this.SourceButtons[this.SourceButton.SelectedItem];
            var sort = this.ZOrderButtons[this.ZOrderButton.SelectedItem];

            if (this.RequestTypeButton.SelectedItem == this.BrowseButton)
                return new BasicQuestionSource() {
                    API = api,
                    Sort = sort,
                    Order = Order.Descending
                };
            else if (this.SearchPicker.SelectedItem == this.InQuestionsButton) {
                if (this.SearchBox.Text == "") return null;
                return null; //TODO: Search question source
            }
            else if (this.SearchPicker.SelectedItem == this.ByAuthorButton) {
                if (this.SearchBox.Text == "") return null;
                return new AuthorQuestionSource() {
                    AuthorName = this.SearchBox.Text,
                    API = api,
                    Sort = sort,
                    Order = Order.Descending
                };
            }
            else return null; //TODO: Activity source
        }

        protected void BeginLoadQuestions(Action<IEnumerable<Question>> success, Action<Exception> error)
        {
            if (this.QuestionSource != null) this.QuestionSource.Abort();

            this.QuestionSource = this.BuildQuestionSource();

            if (this.QuestionSource == null) error(new Exception("Couldn't build question source"));
            else this.QuestionSource.BeginFetchMoreQuestions(count => success(this.QuestionSource.AllQuestions), error);
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

        protected void CreateMappers(IEnumerable<Question> questions) {
            this.CreateRAndThetaMappers(questions);
            this.CreateZMapper(questions);

            this.QuestionMapper = new Func<Question, Vector3>(q => {
                var z = this.ZMapper(q) * 3000 - 3000;
                var r = (1 - this.RMapper(q)) * 1000;
                var theta = this.ThetaMapper(q);

                return new Vector3(r * (float)Math.Cos(theta), r * (float)Math.Sin(theta), z);
            });
        }

        protected void ReMap() {
            this.CreateRAndThetaMappers(this.Questions.Select(qis => qis.Question));
            this.Questions.ForEach(qis => {

                Animator.Animations.Add(new Animation(qis, "Position", 
                    this.QuestionMapper(qis.Question),
                    new TimeSpan(0, 0, 1), Interpolators.QuadraticInOut));
            });
        }

        protected void Repopulate(IEnumerable<Question> rawQuestions)
        {
            if (rawQuestions.Count() == 0) return;

            this.CreateMappers(rawQuestions);

            var questionsInSpace = rawQuestions.Select(q => new QuestionInSpace() {
                Question = q,
                Position = this.QuestionMapper(q),
                Size = this.QuestionFont.MeasureString(q.Title),
                Text = this.VectorQuestionFont.Fill(q.Title) //TODO: Drop if using sprite fonts only
            }).Union(this.Questions).ToList();

            lock (this.Questions) this.Questions = questionsInSpace;

#if false
            Avoid overlap!! :-)
            var world = new World(new Vector2(), true);
            var boxes = new Dictionary<Body, QuestionInSpace>();

            var createBox = new Func<Vector2, Vector2, BodyType, Body>((pos, size, type) => {
                var body = world.CreateBody(new BodyDef() {
                    type = type,
                    position = pos,
                });

                var shape = new PolygonShape();
                shape.SetAsBox(size.X, size.Y);

                body.CreateFixture(new FixtureDef() {
                    shape = shape,
                    density = 1f,
                    friction = 0f
                });

                return body;
            });

            foreach (var question in questions) {
                var idealPos = new Vector2(question.Position.X, question.Position.Y);

                var ideal = createBox(idealPos, new Vector2(0.1f, 0.1f), BodyType.Static);
                var box = createBox(idealPos, new Vector2(question.Size.X, question.Size.Y + 20f), BodyType.Dynamic);

                world.CreateJoint(new DistanceJointDef() {
                    bodyA = ideal,
                    bodyB = box,
                    dampingRatio = 0.5f,
                    frequencyHz = 0.02f,
                    length = 0,
                    localAnchorA = new Vector2(),
                    localAnchorB = new Vector2()
                });

                boxes.Add(box, question);
            }

            stw.Stop();
            Debug.Print("Built world in " + stw.Elapsed.ToString());
            stw.Reset();
            stw.Start();

            for (var i = 0; i < 30; ++i)
                world.Step(1f / 100f, 10, 8);

            stw.Stop();
            Debug.Print("Computed world in " + stw.Elapsed.ToString());

            foreach (var boxQuestion in boxes)
                boxQuestion.Value.Position = new Vector3(
                    boxQuestion.Key.Position.X,
                    boxQuestion.Key.Position.Y,
                    boxQuestion.Value.Position.Z);

            lock (this.Questions)
                this.Questions.AddRange(boxes.Values);
#endif
        }

        protected void ExpandPopulation(IEnumerable<Question> rawQuestions) {
            if (rawQuestions.Count() == 0) return;

            var questionsInSpace = rawQuestions.Select(q => new QuestionInSpace() {
                Question = q,
                Position = this.QuestionMapper(q),
                Size = this.QuestionFont.MeasureString(q.Title),
                Text = this.VectorQuestionFont.Fill(q.Title) //TODO: Drop if using sprite fonts only
            }).Union(this.Questions).ToList();

            lock (this.Questions) this.Questions.AddRange(questionsInSpace);
        }
    }
}
