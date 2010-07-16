using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpaceOverflow.Effects
{
    public static class Extensions
    {
        public static Animation Animate(this object target, string property, object to) {
            return target.Animate(property, null, to);
        }

        public static Animation Animate(this object target, string property, object from, object to) {
            return target.Animate(property, from, to, new TimeSpan(0, 0, 1));
        }

        public static Animation Animate(this object target, string property, object to, TimeSpan duration) {
            return target.Animate(property, null, to, duration);
        }

        public static Animation Animate(this object target, string property, object from, object to, TimeSpan duration) {
            return target.Animate(property, from, to, duration, Interpolators.Linear);
        }

        public static Animation Animate(this object target, string property, object to, TimeSpan duration, Interpolator interpolator) {
            return target.Animate(property, null, to, duration, Interpolators.Linear);
        }

        public static Animation Animate(this object target, string property, object from, object to, TimeSpan duration, Interpolator interpolator) {
            var animation = new Animation() {
                TargetObject = target,
                TargetPropertyName = property,
                From = from,
                To = to,
                Duration = duration,
                Interpolator = interpolator
            };
            Animator.Animations.Add(animation);
            return animation;
        }

        public static Animation Animate(this object target, string property, object to, TimeSpan duration, Interpolator interpolator, Action finished) {
            return target.Animate(property, null, to, duration, Interpolators.Linear, finished);
        }

        public static Animation Animate(this object target, string property, object from, object to, TimeSpan duration, Interpolator interpolator, Action finished) {
            var animation = new Animation(finished) {
                TargetObject = target,
                TargetPropertyName = property,
                From = from,
                To = to,
                Duration = duration,
                Interpolator = interpolator
            };
            Animator.Animations.Add(animation);
            return animation;
        }
    }
}
