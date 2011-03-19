using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.world.space
{
    class FractalCreatorStraight : AFractalCreator
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
                            int primaryCoord = rci.EdgePts[d.RotationCCW][d];
                            int secondaryCoord = rci.EdgePts[d][d.RotationCCW];
                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point ccw = new Point(primaryCoord, secondaryCoord);

                            primaryCoord = rci.EdgePts[d.RotationCW][d];
                            secondaryCoord = rci.EdgePts[d][d.RotationCW];
                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point cw = new Point(primaryCoord, secondaryCoord);

                            addLine(rci, ccw, cw);

                            //rci.InnerPts[d][d.RotationCCW] = ccw;
                            //rci.InnerPts[d][d.RotationCW] = cw;
                        }
                        break;
                    case Connection.Door:

                        // lines from door sides towards screen center
                        foreach (Direction rot in d.Sides)
                        {
                            int primaryCoord = rci.OutsidePts[d];
                            int secondaryCoord = rci.DoorPts[d][rot];
                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point start = new Point(primaryCoord, secondaryCoord);

                            primaryCoord = rci.EdgePts[rot][d];
                            secondaryCoord = rci.DoorPts[d][rot];
                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point end = new Point(primaryCoord, secondaryCoord);

                            if (rot.IsCCWOf(d)) swap<Point>(ref start, ref end);
                            addLine(rci, start, end);
                        }

                        // ends of those lines towards outer edges
                        foreach (Direction rot in d.Sides)
                        {
                            int primaryCoord = rci.EdgePts[rot][d];
                            int secondaryCoord = rci.DoorPts[d][rot];
                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point start = new Point(primaryCoord, secondaryCoord);

                            primaryCoord = rci.EdgePts[rot][d];
                            secondaryCoord = rci.EdgePts[d][rot];
                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point end = new Point(primaryCoord, secondaryCoord);

                            if (rot.IsCCWOf(d)) swap<Point>(ref start, ref end);
                            addLine(rci, start, end);
                        }

                        break;
                    case Connection.Open:
                        foreach (Direction rot in d.Sides)
                        {
                            int primaryCoord = rci.OutsidePts[d];
                            int secondaryCoord = rci.EdgePts[d][rot];
                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point start = new Point(primaryCoord, secondaryCoord);

                            primaryCoord = rci.EdgePts[rot][d];
                            secondaryCoord = rci.EdgePts[d][rot];
                            if (d.IsVertical) swap(ref primaryCoord, ref secondaryCoord);
                            Point end = new Point(primaryCoord, secondaryCoord);

                            if (rot.IsCCWOf(d)) swap<Point>(ref start, ref end);
                            addLine(rci, start, end);
                        }
                        break;
                }
            }

            rci.Fractals = sortFractals(rci.Fractals);
        }
    }
}
