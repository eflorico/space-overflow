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
        QuestionsRequestBase CurrentRequest;

        protected void ReloadAndPopulate() {
            this.ProgressLabel.Text = "Loading...";
            this.ProgressIndicator.IsVisible = true;

            this.Implode();

            try 
            {
                this.BeginLoadQuestions(new Action<DataResponse<Question>>(response => {
                    lock (this.Questions) this.Questions.Clear();
                    this.Repopulate(response.Items);
                    this.Explode();
                    if (this.Questions.Count == 0) this.ProgressLabel.Text = "No questions found";
                    else this.ProgressLabel.Text = "Ready";
                    this.ProgressIndicator.IsVisible = false;
                }));
            }
            catch (Exception ex) {
                Debug.Print("Error while loading quetions:");
                Debug.Print(ex.ToString());

                this.ProgressLabel.Text = "Error";
                this.ProgressIndicator.IsVisible = false;
            }
        }

        protected void LoadAndExpand() {
            this.ProgressLabel.Text = "Loading...";
            this.ProgressIndicator.IsVisible = true;

            try {
                this.CurrentRequest.BeginGetResponse(response => {
                    try {
                        this.ExpandPopulation(response.Items);
                        if (this.Questions.Count == 0) this.ProgressLabel.Text = "No questions found";
                        else this.ProgressLabel.Text = "Ready";
                        this.ProgressIndicator.IsVisible = false;
                    }
                    catch (Exception ex) {
                        Debug.Print("Error while loading quetions:");
                        Debug.Print(ex.ToString());

                        this.ProgressLabel.Text = "Error";
                        this.ProgressIndicator.IsVisible = false;
                    }
                });
            }
            catch (Exception ex) {
                Debug.Print("Error while loading quetions:");
                Debug.Print(ex.ToString());

                this.ProgressLabel.Text = "Error";
                this.ProgressIndicator.IsVisible = false;
            }
        }

        protected void BeginLoadQuestions( Action<DataResponse<Question>> callback)
        {
            StackAPI api = null;

            if (this.SourceButton.SelectedItem == this.StackOverflowButton) api = StackAPI.StackOverflow;
            else if (this.SourceButton.SelectedItem == this.ServerFaultButton) api = StackAPI.ServerFault;
            else if (this.SourceButton.SelectedItem == this.SuperUserButton) api = StackAPI.SuperUser;
            else if (this.SourceButton.SelectedItem == this.MetaButton) api = StackAPI.Meta;
            else if (this.SourceButton.SelectedItem == this.StackAppsButton) api = StackAPI.StackApps;

            if (this.CurrentRequest != null) {
                this.CurrentRequest.Abort();
                this.CurrentRequest = null;
            }

            if (this.RequestTypeButton.SelectedItem == this.BrowseButton) {
                var sort = QuestionSort.Creation;

                if (this.ZOrderButton.SelectedItem == this.ZCreationButton) sort = QuestionSort.Creation;
                else if (this.ZOrderButton.SelectedItem == this.ZFeaturedButton) sort = QuestionSort.Featured;
                else if (this.ZOrderButton.SelectedItem == this.ZVotesButton) sort = QuestionSort.Votes;
                else if (this.ZOrderButton.SelectedItem == this.ZHotButton) sort = QuestionSort.Hot;
                else if (this.ZOrderButton.SelectedItem == this.ZActiveButton) sort = QuestionSort.Activity;

                this.CurrentRequest = new QuestionsRequest(api) {
                    Sort = sort
                };
            }
            else {
                if (this.SearchPicker.SelectedItem == this.InQuestionsButton) {
                    if (this.SearchBox.Text == "") throw new Exception("Empty search!");
                    this.CurrentRequest = new SearchRequest(api) {
                        InTitle = this.SearchBox.Text
                    };
                }
                else if (this.SearchPicker.SelectedItem == this.ByAuthorButton) {
                    new UsersRequest(api) {
                        Filter = this.SearchBox.Text,
                        PageSize = 100
                    }.BeginGetResponse(response => {
                        var user = response.Items.OrderByDescending(item => {
                            if (item.DisplayName.ToLower() == this.SearchBox.Text.ToLower()) return 1;
                            else if (item.DisplayName.ToLower().Contains(this.SearchBox.Text.ToLower())) return 0.5;
                            else return 0;
                        });

                        this.CurrentRequest = new UsersQuestionsRequest(api) {
                            UserID = user.First().ID,
                            Sort = QuestionSort.Creation,
                            Page = 1,
                            PageSize = 100
                        };

                        this.CurrentRequest.BeginGetResponse(callback);
                    });
                }
            }

            if (this.CurrentRequest != null) {
                this.CurrentRequest.PageSize = 100;
                this.CurrentRequest.Page = 1;

                this.CurrentRequest.BeginGetResponse(callback);
            }
        }

        protected void CreateMappers(IEnumerable<Question> questions) {
            Func<Question, float> zCriterionSelector = null;
            float minZ, maxZ;
            int counter = 0;

            if (this.ZOrderButton.SelectedItem == this.ZCreationButton) zCriterionSelector = new Func<Question, float>(q => (float)q.CreationDate.ToUnixTimestamp());
            else if (this.ZOrderButton.SelectedItem == this.ZFeaturedButton) zCriterionSelector = new Func<Question, float>(q =>
                counter++);
            else if (this.ZOrderButton.SelectedItem == this.ZVotesButton) zCriterionSelector = new Func<Question, float>(q => q.UpVoteCount - q.DownVoteCount);
            else if (this.ZOrderButton.SelectedItem == this.ZHotButton) zCriterionSelector = new Func<Question, float>(q => counter++);
            else if (this.ZOrderButton.SelectedItem == this.ZActiveButton) zCriterionSelector = new Func<Question, float>(q => q.LastActivityDate.Ticks);

            if (this.ZOrderButton.SelectedItem == this.ZFeaturedButton || this.ZOrderButton.SelectedItem == this.ZHotButton) {
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
                var range = (float)(maxZ - minZ) ;
                var ret = relativeValue / range;
                return ret;
            });


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

            this.QuestionMapper = new Func<Question, Vector3>(q => {
                var z = this.ZMapper(q) * 3000 - 3000;
                var r = (1 - this.RMapper(q)) * 500;
                var theta = this.ThetaMapper(q);

                return new Vector3(r * (float)Math.Cos(theta), r * (float)Math.Sin(theta), z);
            });
        }

        protected void ReMap() {
            this.CreateMappers(this.Questions.Select(qis => qis.Question));
            this.Questions.ForEach(qis => {
                Animator.Animations.Add(new Animation(qis, "Position", this.QuestionMapper(qis.Question), new TimeSpan(0, 0, 1), Interpolators.QuadraticInOut));
            });
        }

        protected void Repopulate(IEnumerable<Question> rawQuestions)
        {
            if (rawQuestions.Count() == 0) return;

            this.CreateMappers(rawQuestions);

            var questionsInSpace = rawQuestions.Select(q => new QuestionInSpace() {
                Question = q,
                Position = this.QuestionMapper(q),
                Size = this.SpriteQuestionFont.MeasureString(q.Title),
                Text = this.VectorQuestionFont.Fill(q.Title)
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
                Size = this.SpriteQuestionFont.MeasureString(q.Title),
                Text = this.VectorQuestionFont.Fill(q.Title)
            }).Union(this.Questions).ToList();

            lock (this.Questions) this.Questions.AddRange(questionsInSpace);
        }
    }
}
