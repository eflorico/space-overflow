using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SpaceOverflow.Effects
{
    /// <summary>
    /// Updates Animations based on an XNA GameTime.
    /// </summary>
    public static class Animator
    {
        static Animator() {
            Animations = new List<Animation>();
        }

        public static List<Animation> Animations { get; private set; }

        public static void Update(GameTime gameTime) {
            for (var i = 0; i < Animations.Count; ++i)
                if (Animations[i].State != AnimationState.Finished) Animations[i].Update(gameTime);
                else Animations.RemoveAt(i--);
        }
    }
}