using Microsoft.Xna.Framework;
using StackExchange;
using Nuclex.Fonts;

namespace SpaceOverflow
{
    public class QuestionInSpace 
    {
        public Question Question { get; set; }
        public Vector3 Position { get; set; }
        public Vector2 Size { get; set; }
        public Text Text { get; set; }

        public BoundingBox BoundingBox {
            get {
                var bottomLeft = new Vector3(this.Position.X, this.Position.Y, this.Position.Z);
                var topRight = bottomLeft + new Vector3(this.Size, 0);
                return new BoundingBox(bottomLeft, topRight);
            }
        }
    }
}
