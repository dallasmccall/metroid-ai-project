using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA
{
    public struct LineSegment
    {
        public Vector2 p;
        public Vector2 q;

        public LineSegment(Vector2 p, Vector2 q)
        {
            
            this.p = p;
            this.q = q;

        }

        public float getLengthSquared()
        {
            return (p - q).LengthSquared();
        }

        public float getLength()
        {
            return (p - q).Length();
        }

        public Line getLine()
        {
            float m = (p.Y - q.Y)/(p.X - q.X);
            if (q.Y - p.Y == 0) m = 0;
            if (q.X - p.X == 0) m = float.MaxValue;
            float b = p.Y - m*p.X;
            return new Line(m, b);
        }

        public float getX0()
        {
            return Math.Min(p.X, q.X);
        }

        public float getX1()
        {
            return Math.Max(p.X, q.X);
        }

        public bool containsX(float x)
        {
            return (x >= getX0() && x <= getX1());
        }

        public Vector2 getMidpoint()
        {
            return (p + q) / 2;
        }

        public bool getIntersect(ref LineSegment rhs, out Vector2 pt)
        {
            Line l1 = this.getLine();
            Line l2 = rhs.getLine();

            pt = l1.getIntersect(l2);

            if (this.containsX(pt.X) && rhs.containsX(pt.X))
            {
                return true;
            }
            return false;
        }
    }
}
