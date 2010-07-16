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

            this.LastPoll = DateTime.Now;
            this.PendingChanges.Clear();
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

        void Repopulate(IEnumerable<Question> rawQuestions)
        {
            if (rawQuestions.Count() == 0) return;

            this.CreateMappers(rawQuestions);

            var questionsInSpace = rawQuestions.Select(q => this.MapQuestion(q)).OrderBy(qis => qis.Position.Z).ToList();

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

            var questionsInSpace = rawQuestions.Select(q => this.MapQuestion(q)).Union(this.Questions).OrderBy(qis => qis.Position.Z).ToList();

            lock (this.Questions) this.Questions.AddRange(questionsInSpace);
        }

        
    }
}
