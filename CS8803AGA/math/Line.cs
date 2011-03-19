using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA
{
    public struct Line
    {
        public float m;
        public float b;

        public Line(float m, float b)
        {
            this.m = m;
            this.b = b;
        }

        public Vector2 getIntersect(Line rhs)
        {
            // TODO handle same line and parallel lines, horizontal and vertical lines
            float xInter = -(this.b - rhs.b) / (this.m - rhs.m);
            float yInter = this.m * xInter + this.b;
            return new Vector2(xInter, yInter);
        }

        public float getY(float x)
        {
            return m*x + b;
        }

        public Vector2 getPt(float x)
        {
            return new Vector2(x, getY(x));
        }

        public Vector2 getPerpVector()
        {
            if (m == 0)
            {
                return new Vector2(0, 1);
            }
            else if (m == float.MaxValue)
            {
                return new Vector2(1, 0);
            }
            else
            {
                return new Vector2(-m, 1);
            }
        }

        /*
        public Line getPerpenidcular(Vector2 pt)
        {
            if (m == 0)
            {
                // TODO figure out the behavior we want here
                return new Line(float.PositiveInfinity, pt.X);
            }
            else if (m == float.PositiveInfinity)
            {
                return new Line(0, pt.Y);
            }
            else
            {
                float perpSlope = -1.0f/m;
                return new Line(perpSlope, pt.Y - perpSlope * pt.X);
            }
        }
        */
    }
}
