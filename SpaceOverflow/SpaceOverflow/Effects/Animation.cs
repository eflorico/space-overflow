using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace SpaceOverflow.Effects
{
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
            this.TargetProperty = new PropertyInstanceDescriptor(targetObject, targetPropertyName);
            this.From = from;
            this.To = to;
            this.Duration = duration;
            this.Interpolator = interpolator;
        }

        public object From { get; set; }
        public object To { get; set; }
        public TimeSpan Duration { get; set; }
        public Interpolator Interpolator { get; set; }
        public object TargetObject { get; set; }
        public string TargetPropertyName { get; set; }

        public AnimationState State { get; protected set; }

        protected TimeSpan StartTime { get; set; }
        protected object ActualFrom { get; set; }
        protected PropertyInstanceDescriptor TargetProperty { get; set; }

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
                    this.State = AnimationState.Finished;
                    this.TargetProperty.Value = this.To;

                    if (this.Finished != null) this.Finished(this, EventArgs.Empty);
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

    public delegate object Interpolator(object from, object to, float progress);
}
