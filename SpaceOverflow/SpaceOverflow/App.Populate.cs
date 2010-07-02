using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Box2D.XNA;
using StackExchange;
using System.Threading;
using System.Diagnostics;

namespace SpaceOverflow
{
    partial class App
    {
        protected void Repopulate() {
            this.Implode();

            if (this.LoadingThread != null) this.LoadingThread.Abort();

            this.LoadingThread = new Thread(new ThreadStart(() => {
                try {
                    var questions = this.LoadQuestions(1);
                    lock (this.Questions) this.Questions.Clear();
                    this.Populate(questions);
                    this.Explode();
                }
                catch (Exception ex) {
                    //TODO: Add exception handling
                }
            }));

            this.LoadingThread.Start();
        }

        protected void ExpandPopulation() {

        }

        protected IEnumerable<Question> LoadQuestions(int page)
        {
            StackAPI api = null;

            if (this.SourceButton.SelectedItem == this.StackOverflowButton) api = StackAPI.StackOverflow;
            else if (this.SourceButton.SelectedItem == this.ServerFaultButton) api = StackAPI.ServerFault;
            else if (this.SourceButton.SelectedItem == this.SuperUserButton) api = StackAPI.SuperUser;
            else if (this.SourceButton.SelectedItem == this.MetaButton) api = StackAPI.Meta;
            else if (this.SourceButton.SelectedItem == this.StackAppsButton) api = StackAPI.StackApps;

            QuestionsRequestBase request = null;

            if (this.RequestTypeButton.SelectedItem == this.BrowseButton) {
                var sort = QuestionSort.Creation;

                if (this.BrowseOptions.SelectedItem == this.CreationButton) sort = QuestionSort.Creation;
                else if (this.BrowseOptions.SelectedItem == this.FeaturedButton) sort = QuestionSort.Featured;
                else if (this.BrowseOptions.SelectedItem == this.VotesButton) sort = QuestionSort.Votes;
                else if (this.BrowseOptions.SelectedItem == this.HotButton) sort = QuestionSort.Hot;
                else if (this.BrowseOptions.SelectedItem == this.ActiveButton) sort = QuestionSort.Activity;

                request = new QuestionsRequest(api) {
                    Sort = sort
                };
            }
            else {
                if (this.SearchPicker.SelectedItem == this.InQuestionsButton)
                    request = new SearchRequest(api) {
                        InTitle = this.SearchBox.Text
                    };
                else if (this.SearchPicker.SelectedItem == this.ByAuthorButton) {
                    var user = new UsersRequest(api) {
                        Filter = this.SearchBox.Text,
                        PageSize = 100
                    }.GetResponse().Items.OrderByDescending(item => {
                        if (item.DisplayName.ToLower() == this.SearchBox.Text.ToLower()) return 1;
                        else if (item.DisplayName.ToLower().Contains(this.SearchBox.Text.ToLower())) return 0.5;
                        else return 0;
                    });

                    if (user.Count() == 0) throw new Exception("User not found!");

                    request = new UsersQuestionsRequest(api) {
                        UserID = user.First().ID,
                        Sort = QuestionSort.Creation
                    };
                }
            }

            request.PageSize = 100;
            request.Page = page;
            return request.GetResponse().Items;
        }

        protected void Populate(IEnumerable<Question> rawQuestions)
        {
            if (rawQuestions.Count() == 0) return;

            var stw = new Stopwatch();
            stw.Start();

            //Create mappers
            var minDate = rawQuestions.Min(q => q.CreationDate);
            var maxDate = rawQuestions.Max(q => q.CreationDate);
            var minVotes = rawQuestions.Min(q => q.UpVoteCount - q.DownVoteCount);
            var maxVotes = rawQuestions.Max(q => q.UpVoteCount - q.DownVoteCount);

            stw.Stop();
            Debug.Print("Extremes found in " + stw.Elapsed.ToString());
            stw.Reset();
            stw.Start();

            var mapDate = new Func<DateTime, float>(source =>
                (maxDate - minDate).Ticks > 0 ? (float)(source - minDate).Ticks / (float)(maxDate - minDate).Ticks : 1);
            var mapVotes = new Func<int, float>(source => 
                maxVotes - minVotes > 0 ? (float)(source - minVotes) / (float)(maxVotes - minVotes): 1);

            stw.Stop();
            Debug.Print("Mappers initialized in " + stw.Elapsed.ToString());
            stw.Reset();
            stw.Start();

            var pp = this.GraphicsDevice.PresentationParameters;
            var width = pp.BackBufferWidth;
            var height = pp.BackBufferHeight;
            var depth = 3000;

            var angles = new List<float>(rawQuestions.Count());

            stw.Stop();
            Debug.Print("Further setup  in " + stw.Elapsed.ToString());
            stw.Reset();
            stw.Start();

            

            var angleStep = (float)(Math.PI * 2d / (double)angles.Capacity);
            for (var i = 0f; i < angles.Capacity; ++i)
                angles.Add(angleStep * i);

            var gen = new Random(176575);
            angles = angles.OrderBy(x => gen.Next()).ToList();

            stw.Stop();
            Debug.Print("Angles computed in " + stw.Elapsed.ToString());
            stw.Reset();
            stw.Start();

            //Create text blocks and calculate *ideal* positions
            var questions = rawQuestions.Select(q => {
                var r = (1 - mapVotes(q.UpVoteCount - q.DownVoteCount)) * 500;
                var angleIndex = angles.Count > 1 ? q.ID % (angles.Count - 1) : 0;

                var qIS = new QuestionInSpace() {
                    Question = q,
                    Position = new Vector3(
                        r * (float)Math.Cos(angles[angleIndex]),
                        r * (float)Math.Sin(angles[angleIndex]),
                        mapDate(q.CreationDate) * depth - depth),
                    Size = this.SpriteQuestionFont.MeasureString(q.Title),
                    Text = this.VectorQuestionFont.Fill(q.Title)
                };

                angles.RemoveAt(angleIndex);

                return qIS;
            });

            stw.Stop();
            Debug.Print("Ideal positions computed in " + stw.Elapsed.ToString());
            stw.Reset();
            stw.Start();

            //Avoid overlap!! :-)
            //var world = new World(new Vector2(), true);
            //var boxes = new Dictionary<Body, QuestionInSpace>();

            //var createBox = new Func<Vector2, Vector2, BodyType, Body>((pos, size, type) => {
            //    var body = world.CreateBody(new BodyDef() {
            //        type = type,
            //        position = pos,
            //    });

            //    var shape = new PolygonShape();
            //    shape.SetAsBox(size.X, size.Y);

            //    body.CreateFixture(new FixtureDef() {
            //        shape = shape,
            //        density = 1f,
            //        friction = 0f
            //    });

            //    return body;
            //});

            //foreach (var question in questions) {
            //    var idealPos = new Vector2(question.Position.X, question.Position.Y);

            //    var ideal = createBox(idealPos, new Vector2(0.1f, 0.1f), BodyType.Static);
            //    var box = createBox(idealPos, new Vector2(question.Size.X, question.Size.Y + 20f), BodyType.Dynamic);

            //    world.CreateJoint(new DistanceJointDef() {
            //        bodyA = ideal,
            //        bodyB = box,
            //        dampingRatio = 0.5f,
            //        frequencyHz = 0.02f,
            //        length = 0,
            //        localAnchorA = new Vector2(),
            //        localAnchorB = new Vector2()
            //    });

            //    boxes.Add(box, question);
            //}

            //stw.Stop();
            //Debug.Print("Built world in " + stw.Elapsed.ToString());
            //stw.Reset();
            //stw.Start();

            //for (var i = 0; i < 30; ++i)
            //    world.Step(1f / 100f, 10, 8);

            //stw.Stop();
            //Debug.Print("Computed world in " + stw.Elapsed.ToString());

            //foreach (var boxQuestion in boxes)
            //    boxQuestion.Value.Position = new Vector3(
            //        boxQuestion.Key.Position.X,
            //        boxQuestion.Key.Position.Y,
            //        boxQuestion.Value.Position.Z);

            //lock (this.Questions)
            //    this.Questions.AddRange(boxes.Values);
            lock (this.Questions) this.Questions.AddRange(questions);
        }
    }
}
