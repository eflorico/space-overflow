using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SpaceOverflow
{
    public class Curve3D
    {
        public Curve X = new Curve();
        public Curve Y = new Curve();
        public Curve Z = new Curve();

        public void AddPoint(Vector3 point, float time) {
            this.X.Keys.Add(new CurveKey(time, point.X));
            this.Y.Keys.Add(new CurveKey(time, point.Y));
            this.Z.Keys.Add(new CurveKey(time, point.Z));
        }

        public void SetTangents() {
            CurveKey prev;
            CurveKey current;
            CurveKey next;
            int prevIndex;
            int nextIndex;
            for (int i = 0; i < this.X.Keys.Count; i++) {
                prevIndex = i - 1;
                if (prevIndex < 0) prevIndex = i;

                nextIndex = i + 1;
                if (nextIndex == this.X.Keys.Count) nextIndex = i;

                prev = this.X.Keys[prevIndex];
                next = this.X.Keys[nextIndex];
                current = this.X.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                this.X.Keys[i] = current;

                prev = this.Y.Keys[prevIndex];
                next = this.Y.Keys[nextIndex];
                current = this.Y.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                this.Y.Keys[i] = current;

                prev = this.Z.Keys[prevIndex];
                next = this.Z.Keys[nextIndex];
                current = this.Z.Keys[i];
                SetCurveKeyTangent(ref prev, ref current, ref next);
                this.Z.Keys[i] = current;
            }
        }

        static void SetCurveKeyTangent(ref CurveKey prev, ref CurveKey cur,
            ref CurveKey next) {
            float dt = next.Position - prev.Position;
            float dv = next.Value - prev.Value;

            cur.TangentIn = Math.Abs(cur.Value - prev.Value) / 2;
            cur.TangentOut = Math.Abs(next.Value - cur.Value) / 2;

            if (Math.Abs(dv) < float.Epsilon) {
                cur.TangentIn = 0;
                cur.TangentOut = 0;
            }
            else {
                // The in and out tangents should be equal to the 
                // slope between the adjacent keys.
                cur.TangentIn = dv * (cur.Position - prev.Position) / dt;
                cur.TangentOut = dv * (next.Position - cur.Position) / dt;
            }
        }
    }
}
