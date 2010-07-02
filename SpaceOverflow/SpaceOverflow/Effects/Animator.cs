using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceOverflow.Effects
{
    public class Animator
    {
        public Animator() {
            this.Animations = new List<Animation>();
        }

        public List<Animation> Animations { get; private set; }

        public void Update(GameTime gameTime) {
            for( var i = 0; i< this.Animations.Count; ++i)
                if (this.Animations[i].State != AnimationState.Finished) this.Animations[i].Update(gameTime);
                else this.Animations.RemoveAt(i--);
        }
    }
}
