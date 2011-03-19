using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA
{
    public class Fractal
    {
        static Vector2 s_lineFractalPerp;
        static float s_lineFractalLength;

        public static List<LineSegment> fractalLineSegment(LineSegment ls, float maxOffset, float threshold)
        {
            // uncomment these lines to temporary disable line fractals
            //List<LineSegment> dummy = new List<LineSegment>();
            //dummy.Add(ls);
            //return dummy;

            s_lineFractalPerp = ls.getLine().getPerpVector();
            
            s_lineFractalLength = ls.getLength();
            return fractalLineSegment(ls, maxOffset, threshold, 1);
        }

        public static List<LineSegment> fractalLineSegment(LineSegment ls, float baseMaxOffset, float threshold, int depth)
        {
            // TODO fix for short line segments... it goes crazy!  that or increase threshold, but still...

            const int maxDepth = 6;

            List<LineSegment> result = new List<LineSegment>();

            // recursive base case
            if (ls.getLengthSquared() < threshold || depth >= maxDepth)
            {
                result.Add(ls);
                return result;
            }

            // calculate and offset midpt
            // easy approach - midpt
              Vector2 midpt = ls.getMidpoint();
            // other approach - any pt on the line segment selected from a normal dist between the pts
            // for whatever reason, this doesn't seem to work quite as well
            //float percentBetween = RandomManager.nextNormalDistPercent(1);
            //Vector2 midpt = ls.getLine().getPt(ls.p.X + (ls.q.X - ls.p.X) * percentBetween);

            // Can calculate max offset for this segment either as proportion of the original max depth
            //  float maxOffset = maxOffset * (maxDepth - depth) / maxDepth;
            // Or as a proportion of the original length of the line
            float maxOffset = baseMaxOffset * ls.getLength() / s_lineFractalLength;
            float offset = (float)(RandomManager.get().NextDouble() - 0.5f) * maxOffset;

            // Can get offset direction either from current line segment or original line segment
            // Vector2 dir = ls.getLine().getPerpVector();
            Vector2 offsetDir = s_lineFractalPerp;
            offsetDir.Normalize();
            midpt += offset * offsetDir;

            // recurse!
            List<LineSegment> left = fractalLineSegment(new LineSegment(ls.p, midpt), maxOffset, threshold, depth+1);
            List<LineSegment> right = fractalLineSegment(new LineSegment(midpt, ls.q), maxOffset, threshold, depth+1);
            result.AddRange(left);
            result.AddRange(right);
            return result;
        }
    }
}
