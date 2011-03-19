using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI.world.space
{
    abstract class AFractalCreator : IFractalCreator
    {
        #region IFractalCreator Members

        public abstract void CreateFractals(ScreenConstructionInfo sci);

        #endregion

        /// <summary>
        /// Sorts fractals so that when traversing a fractal list and reaching the end of a fractal,
        /// you move to a fractal whose startpoint is that endpoint if it exists.
        /// </summary>
        /// <param name="fractals"></param>
        /// <returns></returns>
        protected static List<LineFractalInfo> sortFractals(List<LineFractalInfo> fractals)
        {
            List<LineFractalInfo> starts = new List<LineFractalInfo>();
            Dictionary<LineFractalInfo, LineFractalInfo> dependencies = new Dictionary<LineFractalInfo, LineFractalInfo>();
            for (int i = 0; i < fractals.Count; ++i)
            {
                LineFractalInfo cur = fractals[i];
                bool dependent = false;
                for (int j = 0; j < fractals.Count; ++j)
                {
                    LineFractalInfo other = fractals[j];

                    // if other fractal's end is current fractal's start
                    // then other fractal leads into this fractal
                    // so we add an edge from that fractal to this fractal in our
                    // dependencies graph
                    if (other.fractal[other.fractal.Count - 1].q == cur.fractal[0].p)
                    {
                        dependencies.Add(other, cur);
                        dependent = true;
                        break;
                    }
                }
                if (!dependent) starts.Add(cur);
            }

            List<LineFractalInfo> orderedList = new List<LineFractalInfo>();
            foreach (LineFractalInfo start in starts)
            {
                orderedList.Add(start);

                LineFractalInfo cur = start;
                LineFractalInfo child = null;
                while (dependencies.TryGetValue(cur, out child))
                {
                    orderedList.Add(child);
                    cur = child;
                }
            }

            return orderedList;
        }

        public static LineFractalInfo createLine(Point start, Point end)
        {
            List<LineSegment> line = new List<LineSegment>();
            line.Add(new LineSegment(new Vector2(start.X, start.Y), new Vector2(end.X, end.Y)));
            return new LineFractalInfo(line);
        }

        /// <summary>
        /// Adds a line to a screen
        /// </summary>
        public static void addLine(ScreenConstructionInfo rci, Point start, Point end)
        {
            if (start == end) return;

            rci.Fractals.Add(createLine(start, end));
        }

        public static LineFractalInfo createFractal(Point start, Point end)
        {
            float maxOffset = 80.0f;
            float threshold = 6000.0f;

            List<LineSegment> fractal =
                Fractal.fractalLineSegment(
                    new LineSegment(new Vector2(start.X, start.Y), new Vector2(end.X, end.Y)),
                    maxOffset,
                    threshold);

            return new LineFractalInfo(fractal);
        }

        /// <summary>
        /// Adds a fractal to a screen
        /// </summary>
        public static void addFractal(ScreenConstructionInfo rci, Point start, Point end)
        {
            if (start == end) return;

            rci.Fractals.Add(createFractal(start, end));
        }

        public static void swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        public static void swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
    }
}
