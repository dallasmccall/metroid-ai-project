using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI.world.space
{
    class FractalCreatorJagged : AFractalCreator
    {
        public override void CreateFractals(ScreenConstructionInfo rci)
        {
            foreach (Direction d in Direction.All)
            {
                Connection conn = Connection.None;
                if (rci.Parameters.Connections.ContainsKey(d)) conn = rci.Parameters.Connections[d];

                switch (conn)
                {
                    case Connection.None:
                        {
                            Point ccw = Zone.SampleOctantPoint(d, d.RotationCCW);
                            Point cw = Zone.SampleOctantPoint(d, d.RotationCW);

                            addFractal(rci, ccw, cw);

                            rci.InnerPts[d][d.RotationCCW] = ccw;
                            rci.InnerPts[d][d.RotationCW] = cw;
                        }
                        break;
                    case Connection.Door:

                        foreach (Direction rot in d.Sides)
                        {
                            // line from door in towards center
                            int primaryCoord = rci.OutsidePts[d];
                            int secondaryCoord = rci.DoorPts[d][rot];

                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point start = new Point(primaryCoord, secondaryCoord);
                            Point end = DirectionUtils.Move(start, d.Opposite, 120);
                            Point secondStart = end;

                            if (rot.IsCCWOf(d)) swap<Point>(ref start, ref end);
                            addLine(rci, start, end);

                            // line from there towards corner
                            primaryCoord = rci.EdgePts[rot][d];
                            secondaryCoord = rci.EdgePts[d][rot];
                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point secondEnd = new Point(primaryCoord, secondaryCoord);

                            if (rot.IsCCWOf(d)) swap<Point>(ref secondStart, ref secondEnd);
                            addFractal(rci, secondStart, secondEnd);

                            if (rot.IsCWOf(d)) swap<Point>(ref secondStart, ref secondEnd);
                            rci.InnerPts[d][rot] = secondStart;
                        }

                        break;
                    case Connection.Open:
                        {
                            int primaryCoord = rci.OutsidePts[d];
                            int secondaryCoord = rci.EdgePts[d][d.RotationCW];

                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point start = new Point(primaryCoord, secondaryCoord);
                            Point end = Zone.SampleOctantPoint(d, d.RotationCW);

                            addFractal(rci, start, end);
                            rci.InnerPts[d][d.RotationCW] = end;
                        }
                        {
                            int primaryCoord = rci.OutsidePts[d];
                            int secondaryCoord = rci.EdgePts[d][d.RotationCCW];

                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point end = new Point(primaryCoord, secondaryCoord);
                            Point start = Zone.SampleOctantPoint(d, d.RotationCCW);

                            addFractal(rci, start, end);
                            rci.InnerPts[d][d.RotationCCW] = start;
                        }
                        break;
                }
            }

            foreach (Direction d in Direction.All)
            {
                Point start = rci.InnerPts[d][d.RotationCW];
                Point end = rci.InnerPts[d.RotationCW][d];

                addFractal(rci, start, end);
            }

            rci.Fractals = sortFractals(rci.Fractals);
        }
    }
}
