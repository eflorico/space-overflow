using Microsoft.Xna.Framework;
using StackExchange;
using Nuclex.Fonts;

namespace SpaceOverflow
{
    public class QuestionInSpace 
    {
        public Question Question { get; set; }
        public Vector3 Position { get; set; }
        public float Scale { get; set; }
        public Vector2 TextSize { get; set; }
        public Text Text { get; set; }

        public Vector3 TopLeft {
            get {
                return new Vector3(this.Position.X - this.TextSize.X * this.Scale / 2, this.Position.Y + this.TextSize.Y * this.Scale / 2, this.Position.Z);
            }
        }

        public Vector3 BottomRight {
            get {
                return new Vector3(this.Position.X + this.TextSize.X * this.Scale / 2, this.Position.Y - this.TextSize.Y * this.Scale / 2, this.Position.Z);
            }
        }

        public BoundingBox BoundingBox {
            get {
                return new BoundingBox(this.TopLeft, this.BottomRight);
            }
        }
    }
}
