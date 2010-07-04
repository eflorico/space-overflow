using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StackExchange;
using System.Threading;
using System.Diagnostics;

namespace SpaceOverflow
{
    partial class App
    {
        List<QuestionInSpace> Questions;
        Func<Question, QuestionInSpace> QuestionMapper;
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
                    this.ExpandPopulation(response.Items);
                    if (this.Questions.Count == 0) this.ProgressLabel.Text = "No questions found";
                    else this.ProgressLabel.Text = "Ready";
                    this.ProgressIndicator.IsVisible = false;
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

                if (this.BrowseOptions.SelectedItem == this.CreationButton) sort = QuestionSort.Creation;
                else if (this.BrowseOptions.SelectedItem == this.FeaturedButton) sort = QuestionSort.Featured;
                else if (this.BrowseOptions.SelectedItem == this.VotesButton) sort = QuestionSort.Votes;
                else if (this.BrowseOptions.SelectedItem == this.HotButton) sort = QuestionSort.Hot;
                else if (this.BrowseOptions.SelectedItem == this.ActiveButton) sort = QuestionSort.Activity;

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
            var minDate = questions.Min(q => q.CreationDate);
            var maxDate = questions.Max(q => q.CreationDate);

            this.ZMapper = new Func<Question, float>(q => {
                if ((maxDate - minDate).Ticks == 0) return 1;
                
                var relativeValue = (float)(q.CreationDate.Ticks - minDate.Ticks);
                var range = (float)(maxDate.Ticks - minDate.Ticks) ;
                var ret = relativeValue / range;
                return ret;
            });

            var minVotes = questions.Min(q => q.UpVoteCount - q.DownVoteCount);
            var maxVotes = questions.Max(q => q.UpVoteCount - q.DownVoteCount);

            this.RMapper = new Func<Question, float>(q =>
                maxVotes - minVotes > 0 ? (float)(q.UpVoteCount - q.DownVoteCount - minVotes) / (float)(maxVotes - minVotes) : 1);

            this.ThetaMapper = new Func<Question, float>(q =>
                q.ID * q.OwnerID % 143268);

            this.QuestionMapper = new Func<Question, QuestionInSpace>(q => {
                var z = this.ZMapper(q) * 3000 - 3000;
                var r = (1 - this.RMapper(q)) * 500;
                var theta = this.ThetaMapper(q);

                return new QuestionInSpace() {
                    Question = q,
                    Position = new Vector3(r * (float)Math.Cos(theta), r * (float)Math.Sin(theta), z),
                    Size = this.SpriteQuestionFont.MeasureString(q.Title),
                    Text = this.VectorQuestionFont.Fill(q.Title)
                };
            });
        }

        protected void Repopulate(IEnumerable<Question> rawQuestions)
        {
            if (rawQuestions.Count() == 0) return;

            var allRawQuestions = rawQuestions.Union(this.Questions.Select(qis => qis.Question));

            this.CreateMappers(allRawQuestions);
            var mappedQuestions = allRawQuestions.Select(q => this.QuestionMapper(q)).ToList();

            lock (this.Questions) this.Questions = mappedQuestions;

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

            var mappedQuestions = rawQuestions.Select(q => this.QuestionMapper(q));

            lock (this.Questions) this.Questions.AddRange(mappedQuestions);
        }
    }
}
