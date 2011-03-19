using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;

namespace CS8803AGA.world.space.postprocessors
{
    class BallLock : IScreenPostProcessor
    {
        protected bool m_initialized;
        protected Direction m_side;
        //protected int m_height;

        public BallLock()
        {
            // nch
        }

        public BallLock(Direction side)
        {
            m_side = side;

            m_initialized = true;
        }

        #region IScreenPostProcessor Members

        public void PostProcess(Zone zone, Point globalScreenCoord)
        {
            if (!m_initialized)
            {
                if (zone.TopLeftPosition == globalScreenCoord)
                {
                    m_side = Direction.Right;
                }
                else
                {
                    m_side = Direction.Left;
                }
            }

            ScreenConstructionInfo sci = zone.ScreenConstructionInfos[globalScreenCoord];

            sci.Parameters.Connections[m_side] = Connection.None;

            // create fractal for edge of wall; fill in everything inside that wall

            int primaryCoord = sci.OutsidePts[m_side.RotationCCW];
            int secondaryCoord = sci.EdgePts[m_side.RotationCCW][m_side];
            if (m_side.IsHorizontal) AFractalCreator.swap(ref primaryCoord, ref secondaryCoord);
            Point start = new Point(primaryCoord, secondaryCoord);

            primaryCoord = sci.OutsidePts[m_side.RotationCW];
            secondaryCoord = sci.EdgePts[m_side.RotationCW][m_side];
            if (m_side.IsHorizontal) AFractalCreator.swap(ref primaryCoord, ref secondaryCoord);
            Point end = new Point(primaryCoord, secondaryCoord);

            LineFractalInfo fractal = AFractalCreator.createFractal(start, end);
            List<Point> wall =
                GridUtils.getGridIntersects(
                    fractal.fractal,
                    Zone.TILE_WIDTH,
                    Zone.TILE_HEIGHT,
                    new Dictionary<Point, CS8803AGA.GridUtils.TileConfigurer>());
            
            foreach (Point p in wall)
            {
                zone.BlockedTiles.Add(new KeyValuePair<Point, Point>(globalScreenCoord, p));
                if (zone.IsInScreen(p)) zone.Tiles[globalScreenCoord][p.X, p.Y] = zone.EnvironmentFillInfo.FILL;

                Point next = m_side.Move(p);
                while (zone.IsInScreen(next))
                {
                    zone.BlockedTiles.Add(new KeyValuePair<Point, Point>(globalScreenCoord, next));
                    zone.Tiles[globalScreenCoord][next.X, next.Y] = zone.EnvironmentFillInfo.FILL;
                    next = m_side.Move(next);
                }
            }

            // create a path for the ball

            primaryCoord = sci.OutsidePts[m_side];
            secondaryCoord = sci.PathPts[m_side];
            if (m_side.IsVertical) AFractalCreator.swap(ref primaryCoord, ref secondaryCoord);
            start = new Point(primaryCoord, secondaryCoord);

            primaryCoord =
                (Math.Abs(sci.EdgePts[m_side.RotationCCW][m_side] - sci.OutsidePts[m_side]) >
                 Math.Abs(sci.EdgePts[m_side.RotationCW][m_side] - sci.OutsidePts[m_side]))
                 ? sci.EdgePts[m_side.RotationCCW][m_side] : sci.EdgePts[m_side.RotationCW][m_side];
            secondaryCoord = sci.PathPts[m_side];
            if (m_side.IsVertical) AFractalCreator.swap(ref primaryCoord, ref secondaryCoord);
            end = new Point(primaryCoord, secondaryCoord);

            LineFractalInfo line = AFractalCreator.createLine(start, end);

            List<Point> path =
                GridUtils.getGridIntersects(
                    line.fractal,
                    Zone.TILE_WIDTH,
                    Zone.TILE_HEIGHT,
                    new Dictionary<Point, CS8803AGA.GridUtils.TileConfigurer>());

            Point endTile = new Point(end.X / Zone.TILE_WIDTH, end.Y / Zone.TILE_HEIGHT);
            zone.TileDebugLabels[globalScreenCoord][endTile] = "End";

            Point insetTile = m_side.Move(endTile);
            zone.TileDebugLabels[globalScreenCoord][insetTile] = "Inset";
            zone.BlockedTiles.Remove(new KeyValuePair<Point, Point>(globalScreenCoord, insetTile));
            zone.Tiles[globalScreenCoord][insetTile.X, insetTile.Y] = zone.EnvironmentFillInfo.EMPTY;

            // clear a 3x3 square above path to guarantee samus can get to the opening
            for (int i = 0; i < 3; ++i)
            {
                Point basePt = m_side.Opposite.Move(endTile, i);
                for (int j = 1; j < 4; ++j)
                {
                    Point cur = Direction.Up.Move(basePt, j);
                    zone.TileDebugLabels[globalScreenCoord][cur] = "Cleared";
                    zone.BlockedTiles.Remove(new KeyValuePair<Point, Point>(globalScreenCoord, cur));
                    if (zone.IsInScreen(cur))
                    {
                        zone.Tiles[globalScreenCoord][cur.X, cur.Y] = zone.EnvironmentFillInfo.EMPTY;
                    }
                }
            }

            // clear opening and ensure that a ledge exists beneath the opening
            foreach (Point p in path)
            {
                zone.FakeTiles.Add(new KeyValuePair<Point, Point>(globalScreenCoord, p));
                //zone.BlockedTiles.Remove(new KeyValuePair<Point, Point>(globalScreenCoord, p));
                //if (zone.IsInScreen(p)) zone.Tiles[globalScreenCoord][p.X, p.Y] = zone.EnvironmentFillInfo.EMPTY;

                Point down = Direction.Down.Move(p);
                zone.BlockedTiles.Add(new KeyValuePair<Point, Point>(globalScreenCoord, down));
                if (zone.IsInScreen(down)) zone.Tiles[globalScreenCoord][down.X, down.Y] = zone.EnvironmentFillInfo.FILL;

                Point down2 = Direction.Down.Move(down);
                zone.BlockedTiles.Add(new KeyValuePair<Point, Point>(globalScreenCoord, down2));
                if (zone.IsInScreen(down2)) zone.Tiles[globalScreenCoord][down2.X, down2.Y] = zone.EnvironmentFillInfo.FILL;
            }
        }

        #endregion
    }
}
