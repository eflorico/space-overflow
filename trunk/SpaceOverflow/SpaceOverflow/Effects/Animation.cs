using System;
using Microsoft.Xna.Framework;

namespace SpaceOverflow.Effects
{
    /// <summary>
    /// Represents an Animation on a field or property of an object. Can be executed with an Animator.
    /// </summary>
    public class Animation
    {
        public Animation(object targetObject, string targetPropertyName, object to) 
            : this(targetObject, targetPropertyName, to, new TimeSpan(0, 0, 0, 1)) { }

        public Animation(object targetObject, string targetPropertyName, object to, TimeSpan duration) 
            : this(targetObject, targetPropertyName, to, duration, Interpolators.Linear) { }

        public Animation(object targetObject, string targetPropertyName, object to, TimeSpan duration, Interpolator interpolator)
            : this(targetObject, targetPropertyName, null, to, duration, interpolator) { }

        public Animation(object targetObject, string targetPropertyName, object from, object to, TimeSpan duration, Interpolator interpolator) {
            this.TargetObject = targetObject;
            this.TargetPropertyName = targetPropertyName;
            this.TargetProperty = new PropertyOrFieldInstanceDescriptor(targetObject, targetPropertyName);
            this.From = from;
            this.To = to;
            this.Duration = duration;
            this.Interpolator = interpolator;
            this.Repetitions = 1;
        }

        public object From { get; set; }
        public object To { get; set; }
        public TimeSpan Duration { get; set; }
        public Interpolator Interpolator { get; set; }
        public object TargetObject { get; set; }
        public string TargetPropertyName { get; set; }
        public int Repetitions { get; set; }

        public AnimationState State { get; protected set; }

        protected TimeSpan StartTime { get; set; }
        protected object ActualFrom { get; set; }
        protected PropertyOrFieldInstanceDescriptor TargetProperty { get; set; }

        public void Update(GameTime gameTime) {
            if (this.State != AnimationState.Animating) { //Begin animation
                this.State = AnimationState.Animating;
                this.StartTime = gameTime.TotalGameTime;
                this.ActualFrom = this.From ?? this.TargetProperty.Value;
            }
            else { //Update animation
                var progress = (float)(gameTime.TotalGameTime - this.StartTime).Ticks / (float)this.Duration.Ticks;
                if (progress < 1) this.TargetProperty.Value = this.Interpolator(this.ActualFrom, this.To, progress);
                else { //Finish animation
                    if (this.Repetitions > 0 && --this.Repetitions == 0) {
                        this.State = AnimationState.Finished;
                        this.TargetProperty.Value = this.To;
                        if (this.Finished != null) this.Finished(this, EventArgs.Empty);
                    }
                    else {
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
