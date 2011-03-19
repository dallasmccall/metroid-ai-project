using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI
{
    /// <summary>
    /// Utility for tile sets to determine what type of tile to draw in a
    /// certain point based on lines moving through them to represent borders.
    /// </summary>
    public class GridUtils
    {
        public enum TILE_SIDE
        {
            UNMARKED, LEFT, RIGHT, TOP, BOTTOM
        }

        public enum TILE_TYPE
        {
            EMPTY, FILL, FILL_LEFT, FILL_RIGHT, FILL_TOP, FILL_BOTTOM, FILL_TOPRIGHT, FILL_BOTTOMRIGHT, FILL_TOPLEFT, FILL_BOTTOMLEFT, EMPTY_TOPRIGHT, EMPTY_BOTTOMRIGHT, EMPTY_TOPLEFT, EMPTY_BOTTOMLEFT
        }

        // class used to keep track of how line segments pass through tiles
        //  so we know what type of artwork to give them
        public class TileConfigurer
        {
            //public Point location;

            public TILE_SIDE entry;
            public TILE_SIDE exit;

            public TileConfigurer()
            {
                //this.location = location;
                this.entry = TILE_SIDE.UNMARKED;
                this.exit = TILE_SIDE.UNMARKED;
            }

            public void markEntry(TILE_SIDE e)
            {
                // reentering tile from where we exited before
                if (exit != TILE_SIDE.UNMARKED && exit == e)
                {
                    exit = TILE_SIDE.UNMARKED;
                    return;
                }
                // reentering tile from somewhere else... don't have art to handle.
                else if (exit != TILE_SIDE.UNMARKED)
                {
                    // for now, we'll overwrite the first pass
                    entry = e;
                    exit = TILE_SIDE.UNMARKED;
                    return;
                }

                entry = e;
            }

            public void markExit(TILE_SIDE e)
            {
                // doubles back... undo tile
                if (e == entry)
                {
                    entry = TILE_SIDE.UNMARKED;
                    exit = TILE_SIDE.UNMARKED;
                    return;
                }

                exit = e;
            }

            public bool isMarked()
            {
                return (entry != TILE_SIDE.UNMARKED || exit != TILE_SIDE.UNMARKED);
            }

            public TILE_TYPE getType()
            {
                if (!isMarked()) return TILE_TYPE.EMPTY;
                //if (entry == TILE_SIDE.UNMARKED || exit == TILE_SIDE.UNMARKED) throw new Exception("Shouldnt be grabbing types of tiles that aren't fully marked");

                // horiz / vert
                if (entry == TILE_SIDE.LEFT && exit == TILE_SIDE.RIGHT) return TILE_TYPE.FILL_TOP;
                if (entry == TILE_SIDE.RIGHT && exit == TILE_SIDE.LEFT) return TILE_TYPE.FILL_BOTTOM;
                if (entry == TILE_SIDE.TOP && exit == TILE_SIDE.BOTTOM) return TILE_TYPE.FILL_RIGHT;
                if (entry == TILE_SIDE.BOTTOM && exit == TILE_SIDE.TOP) return TILE_TYPE.FILL_LEFT;

                // just filled corners
                if (entry == TILE_SIDE.LEFT && exit == TILE_SIDE.TOP) return TILE_TYPE.FILL_TOPLEFT;
                if (entry == TILE_SIDE.TOP && exit == TILE_SIDE.RIGHT) return TILE_TYPE.FILL_TOPRIGHT;
                if (entry == TILE_SIDE.RIGHT && exit == TILE_SIDE.BOTTOM) return TILE_TYPE.FILL_BOTTOMRIGHT;
                if (entry == TILE_SIDE.BOTTOM && exit == TILE_SIDE.LEFT) return TILE_TYPE.FILL_BOTTOMLEFT;

                // three-fourths
                if (entry == TILE_SIDE.LEFT && exit == TILE_SIDE.BOTTOM) return TILE_TYPE.EMPTY_BOTTOMLEFT;
                if (entry == TILE_SIDE.BOTTOM && exit == TILE_SIDE.RIGHT) return TILE_TYPE.EMPTY_BOTTOMRIGHT;
                if (entry == TILE_SIDE.RIGHT && exit == TILE_SIDE.TOP) return TILE_TYPE.EMPTY_TOPRIGHT;
                if (entry == TILE_SIDE.TOP && exit == TILE_SIDE.LEFT) return TILE_TYPE.EMPTY_TOPLEFT;

                return TILE_TYPE.FILL;
            }
        }

        public static List<Point> getGridIntersects(List<LineSegment> segs, int gridWidth, int gridHeight, Dictionary<Point, TileConfigurer> tiles)
        {
            // from http://valis.cs.uiuc.edu/~sariel/research/CG/compgeom/msg00925.html

            // Note that here we are using Points as grid cells, since they are two ints
            List<Point> markedCells = new List<Point>();

            foreach (LineSegment ls in segs)
            {
                Ray2 r = new Ray2(ls.p, ls.q - ls.p);
                Point initCell = new Point((int)r.start.X / gridWidth, (int)r.start.Y / gridHeight);
                if (r.start.X < 0) initCell.X--;
                if (r.start.Y < 0) initCell.Y--;

                float t = 0;
                float tMax;
                if (r.dir.X == 0)
                    tMax = (ls.q.Y - ls.p.Y) / r.dir.Y;
                else if (r.dir.Y == 0)
                    tMax = (ls.q.X - ls.p.X) / r.dir.X;
                else
                    tMax = Math.Min((ls.q.Y - ls.p.Y) / r.dir.Y, (ls.q.X - ls.p.X) / r.dir.X);

                // prevent div by 0 later
                if (r.dir.X == 0)
                    r.dir.X = float.Epsilon;
                if (r.dir.Y == 0)
                    r.dir.Y = float.Epsilon;

                // which way does the ray move through the cells
                float xCellAdj;
                float yCellAdj;
                if (r.dir.X > 0)
                {
                    xCellAdj = gridWidth;
                }
                else
                {
                    xCellAdj = 0;
                }
                if (r.dir.Y > 0)
                {
                    yCellAdj = gridHeight;
                }
                else
                {
                    yCellAdj = 0;
                }

                Point curCell = initCell;
                while (t < tMax) // if we do <= tMax, and the segment ends on the edge of a tile, then we'll actually
                                 // end up carrying that line segment through that tile - bad
                {
                    markedCells.Add(curCell);

                    // find grid cells in direction of ray
                    float nextGridLineX;
                    float nextGridLineY;
                    nextGridLineX = curCell.X * gridWidth + xCellAdj;
                    nextGridLineY = curCell.Y * gridHeight + yCellAdj;

                    // calc time we'll hit each of them and, hit closer
                    float tx = (nextGridLineX - r.start.X) / r.dir.X;
                    float ty = (nextGridLineY - r.start.Y) / r.dir.Y;
                    Point nextCell = curCell;
                    if (tx < ty && tx < tMax)
                    {
                        t = tx + float.Epsilon; // since area border fractals are generated using integer intersection
                                                // pts, a line seg might touch a tile but not go into it... this hopefully
                                                // resolves that problem
                        if (xCellAdj > 0)
                        {
                            if (tiles != null && !tiles.ContainsKey(curCell)) tiles.Add(curCell, new TileConfigurer());
                            if (tiles != null) { tiles[curCell].markExit(TILE_SIDE.RIGHT); }
                            nextCell.X++;
                            if (tiles != null && !tiles.ContainsKey(nextCell)) tiles.Add(nextCell, new TileConfigurer());
                            if (tiles != null) { tiles[nextCell].markEntry(TILE_SIDE.LEFT); }
                        }
                        else
                        {
                            if (tiles != null && !tiles.ContainsKey(curCell)) tiles.Add(curCell, new TileConfigurer());
                            if (tiles != null) { tiles[curCell].markExit(TILE_SIDE.LEFT); }
                            nextCell.X--;
                            if (tiles != null && !tiles.ContainsKey(nextCell)) tiles.Add(nextCell, new TileConfigurer());
                            if (tiles != null) { tiles[nextCell].markEntry(TILE_SIDE.RIGHT); }
                        }
                    }
                    else if (ty < tMax)
                    {
                        t = ty + float.Epsilon; // since area border fractals are generated using integer intersection
                        // pts, a line seg might touch a tile but not go into it... this hopefully
                        // resolves that problem
                        if (yCellAdj > 0)
                        {
                            if (tiles != null && !tiles.ContainsKey(curCell)) tiles.Add(curCell, new TileConfigurer());
                            if (tiles != null) { tiles[curCell].markExit(TILE_SIDE.BOTTOM); }
                            nextCell.Y++;
                            if (tiles != null && !tiles.ContainsKey(nextCell)) tiles.Add(nextCell, new TileConfigurer());
                            if (tiles != null) { tiles[nextCell].markEntry(TILE_SIDE.TOP); }
                        }
                        else
                        {
                            if (tiles != null && !tiles.ContainsKey(curCell)) tiles.Add(curCell, new TileConfigurer());
                            if (tiles != null) { tiles[curCell].markExit(TILE_SIDE.TOP); }
                            nextCell.Y--;
                            if (tiles != null && !tiles.ContainsKey(nextCell)) tiles.Add(nextCell, new TileConfigurer());
                            if (tiles != null) { tiles[nextCell].markEntry(TILE_SIDE.BOTTOM); }
                        }
                    }
                    else
                    {
                        t = tMax; // hack to make loop continue.
                        // basically, if we won't reach a new cell before reaching end of the ray,
                        // just continue.
                    }
                    curCell = nextCell;
                    //r.start = r.evalAtT(t);
                }
            }

            return markedCells;
        }
    }
}
