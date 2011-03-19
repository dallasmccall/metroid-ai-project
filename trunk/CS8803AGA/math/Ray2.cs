using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA
{
    public class Ray2
    {
        public Vector2 start;
        public Vector2 dir;

        public Ray2(Vector2 start, Vector2 dir)
        {
            this.start = start;
            this.dir = dir;
        }

        public Vector2 evalAtT(float t)
        {
            return new Vector2(start.X + t * dir.X, start.Y + t * dir.Y);
        }
    }
}
