using System;
using Microsoft.Xna.Framework;

namespace SpaceOverflow.Effects
{
    /// <summary>
    /// Represents an Animation on a field or property of an object. Can be executed with an Animator.
    /// </summary>
    public class Animation
    {
        public Animation() {
            this.Repetitions = 1;
        }

        public Animation(Action finished) : this() {
            this.Finished += new EventHandler((sender, e) => finished());
        }

        public object From { get; set; }
        public object To { get; set; }
        public TimeSpan Duration { get; set; }
        public Interpolator Interpolator { get; set; }
        public object TargetObject { get; set; }

        private string _targetPropertyName;
        public string TargetPropertyName {
            get {
                return this._targetPropertyName;
            }
            set {
                this.TargetProperty = new PropertyOrFieldInstanceDescriptor(this.TargetObject, value);
                this._targetPropertyName = value;
            }
        }
        public int Repetitions { get; set; }

        public AnimationState State { get; protected set; }

        protected TimeSpan StartTime { get; set; }
        protected object ActualFrom { get; set; }
        protected PropertyOrFieldInstanceDescriptor TargetProperty { get; set; }

        public void Update(GameTime gameTime) {
            //Begin animation
            if (this.State != AnimationState.Animating) { 
                this.State = AnimationState.Animating;
                this.StartTime = gameTime.TotalGameTime;
                this.ActualFrom = this.From ?? this.TargetProperty.Value;
            }
            else { //Update animation
                var progress = (float)(gameTime.TotalGameTime - this.StartTime).Ticks / (float)this.Duration.Ticks;
                if (progress < 1) this.TargetProperty.Value = this.Interpolator(this.ActualFrom, this.To, progress);
                else {
                    if (this.Repetitions > 0 && --this.Repetitions == 0) { //Finish animation
                        this.State = AnimationState.Finished;
                        this.TargetProperty.Value = this.To;
                        if (this.Finished != null) this.Finished(this, EventArgs.Empty);
                    }
                    else { //Repeat
                        this.TargetProperty.Value = this.ActualFrom;
                        this.StartTime = gameTime.TotalGameTime;
                    }
                }
            }
        }

        public void Reset() {
            this.State = AnimationState.Ready;
        }

        public event EventHandler Finished;
    }

    public enum AnimationState
    {
        Ready,
        Animating,
        Finished
    }

    /// <param name="from">The start value.</param>
    /// <param name="to">The end value.</param>
    /// <param name="progress">The progress, ranging from 0 to 1.</param>
    /// <returns>The interpolated value.</returns>
    public delegate object Interpolator(object from, object to, float progress);
}
