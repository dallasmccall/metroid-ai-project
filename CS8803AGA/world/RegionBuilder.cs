using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CS8803AGA;

namespace CS8803AGA
{

    #region Helper Classes

    /// <summary>
    /// Simple class for creating a grid-based world, each marker is a cell in the grid and contains
    /// an environment type and whether it is connected to another cell on each side
    /// </summary>
    public class RegionMarker
    {
        public Point location;

        public bool connectsLeft;
        public bool connectsRight;
        public bool connectsTop;
        public bool connectsBottom;

        public string environment;

        public RegionMarker(Point location, bool top, bool left, bool bottom, bool right, string environment)
        {
            this.location = location;
            this.connectsLeft = left;
            this.connectsRight = right;
            this.connectsTop = top;
            this.connectsBottom = bottom;
            this.environment = environment;
        }
    }

    /// <summary>
    /// Intermediate class for instantiating regions
    /// Contains integers for the points at which borders enter/exit the region; these are
    ///     used as endpoints for fractals so that the borders on adjacent regions/areas connect
    /// </summary>
    public class RegionConstructionInfo
    {
        public Point location;
        public RegionMarker marker;

        public int pathTop = -1;
        public int pathLeft = -1;
        public int pathBottom = -1;
        public int pathRight = -1;

        public int topLeft = -1;
        public int topRight = -1;
        public int leftTop = -1;
        public int leftBottom = -1;
        public int bottomLeft = -1;
        public int bottomRight = -1;
        public int rightTop = -1;
        public int rightBottom = -1;

        public List<LineFractalInfo> fractals;

        public RegionConstructionInfo(RegionMarker marker)
        {
            this.location = marker.location;
            this.marker = marker;
            this.fractals = new List<LineFractalInfo>();
        }
    }

    /// <summary>
    /// Set of line segments forming a line fractal
    /// </summary>
    public class LineFractalInfo
    {
        public List<LineSegment> fractal;

        public LineFractalInfo(List<LineSegment> fractal)
        {
            this.fractal = fractal;
        }
    }

    #endregion

    /// <summary>
    /// Class for constructing regions
    /// </summary>
    public class RegionBuilder
    {
        public const int REGION_WIDTH = Region.CONSTRUCTION_WIDTH;
        public const int REGION_HEIGHT = Region.CONSTRUCTION_HEIGHT;

        /// <summary>
        /// Creates hardcoded world map and registers it
        /// </summary>
        public static void test()
        {
            Dictionary<Point, RegionMarker> markers =
                new Dictionary<Point, RegionMarker>();

            RegionMarker a1n2 = new RegionMarker(new Point(1, -2), false, false, true, false, "town");

            RegionMarker an1n1 = new RegionMarker(new Point(-1, -1), false, false, true, true, "forest");
            RegionMarker a0n1 = new RegionMarker(new Point(0, -1), false, true, false, false, "town");
            RegionMarker a1n1 = new RegionMarker(new Point(1, -1), true, false, false, true, "darkness");
            RegionMarker a2n1 = new RegionMarker(new Point(2, -1), false, true, true, false, "graveyard");

            RegionMarker an10 = new RegionMarker(new Point(-1, 0), true, false, true, false, "town");
            RegionMarker a00 = new RegionMarker(new Point(0, 0), false, false, false, true, "darkness");
            RegionMarker a10 = new RegionMarker(new Point(1, 0), false, true, true, true, "graveyard");
            RegionMarker a20 = new RegionMarker(new Point(2, 0), true, true, true, false, "forest");

            RegionMarker an11 = new RegionMarker(new Point(-1, 1), true, false, true, true, "cave");
            RegionMarker a01 = new RegionMarker(new Point(0, 1), false, true, false, true, "mountain");
            RegionMarker a11 = new RegionMarker(new Point(1, 1), true, true, false, true, "town");
            RegionMarker a21 = new RegionMarker(new Point(2, 1), true, true, false, false, "cotton");

            RegionMarker an12 = new RegionMarker(new Point(-1, 2), true, false, false, true, "castle");
            RegionMarker a02 = new RegionMarker(new Point(0, 2), false, true, true, false, "graveyard");

            RegionMarker a03 = new RegionMarker(new Point(0, 3), true, false, false, false, "cave");

            markers.Add(a1n2.location, a1n2);
            markers.Add(an1n1.location, an1n1);
            markers.Add(a0n1.location, a0n1);
            markers.Add(a1n1.location, a1n1);
            markers.Add(a2n1.location, a2n1);
            markers.Add(an10.location, an10);
            markers.Add(a00.location, a00);
            markers.Add(a10.location, a10);
            markers.Add(a20.location, a20);
            markers.Add(an11.location, an11);
            markers.Add(a01.location, a01);
            markers.Add(a11.location, a11);
            markers.Add(a21.location, a21);
            markers.Add(an12.location, an12);
            markers.Add(a02.location, a02);
            markers.Add(a03.location, a03);

            Dictionary<Point, RegionConstructionInfo> rcis =
                buildConstructionInfo(markers);

            createFractals(rcis);
            Dictionary<Point,Region> regions = createRegions(rcis);
        }

        /// <summary>
        /// Converts RegionMarker's into RegionConstructionInfo's by creating endpoints for boundary lines
        /// </summary>
        /// <param name="markers"></param>
        /// <returns></returns>
        public static Dictionary<Point, RegionConstructionInfo> buildConstructionInfo(Dictionary<Point, RegionMarker> markers)
        {
            Dictionary<Point, RegionConstructionInfo> rcis =
                new Dictionary<Point,RegionConstructionInfo>();

            Random rand = RandomManager.get();

            foreach (RegionMarker marker in markers.Values)
            {
                RegionConstructionInfo rci = new RegionConstructionInfo(marker);
                rcis.Add(rci.location, rci);

                if (marker.connectsTop && rcis.ContainsKey(new Point(marker.location.X, marker.location.Y - 1)))
                {
                    RegionConstructionInfo above = rcis[new Point(marker.location.X, marker.location.Y - 1)];
                    rci.topLeft = above.bottomLeft;
                    rci.topRight = above.bottomRight;
                    rci.pathTop = above.pathBottom;
                }
                else
                {
                    rci.topLeft = rand.Next(0, REGION_WIDTH / 4);
                    rci.topRight = rand.Next(REGION_WIDTH * 3 / 4, REGION_WIDTH);
                    rci.pathTop = rand.Next(REGION_WIDTH / 3, REGION_WIDTH * 2 / 3);
                }

                if (marker.connectsLeft && rcis.ContainsKey(new Point(marker.location.X-1, marker.location.Y)))
                {
                    RegionConstructionInfo left = rcis[new Point(marker.location.X-1, marker.location.Y)];
                    rci.leftTop = left.rightTop;
                    rci.leftBottom = left.rightBottom;
                    rci.pathLeft = left.pathRight;
                }
                else
                {
                    rci.leftTop = rand.Next(0, REGION_HEIGHT / 4);
                    rci.leftBottom = rand.Next(REGION_HEIGHT * 3 / 4, REGION_HEIGHT);
                    rci.pathLeft = rand.Next(REGION_HEIGHT / 3, REGION_HEIGHT * 2 / 3);
                }

                if (marker.connectsBottom && rcis.ContainsKey(new Point(marker.location.X, marker.location.Y+1)))
                {
                    RegionConstructionInfo below = rcis[new Point(marker.location.X, marker.location.Y+1)];
                    rci.bottomLeft = below.topLeft;
                    rci.bottomRight = below.topRight;
                    rci.pathBottom = below.pathTop;
                }
                else
                {
                    rci.bottomLeft = rand.Next(0, REGION_WIDTH / 4);
                    rci.bottomRight = rand.Next(REGION_WIDTH * 3 / 4, REGION_WIDTH);
                    rci.pathBottom = rand.Next(REGION_WIDTH / 3, REGION_WIDTH * 2 / 3);
                }

                if (marker.connectsRight && rcis.ContainsKey(new Point(marker.location.X + 1, marker.location.Y)))
                {
                    RegionConstructionInfo right = rcis[new Point(marker.location.X + 1, marker.location.Y)];
                    rci.rightTop = right.leftTop;
                    rci.rightBottom = right.leftBottom;
                    rci.pathRight = right.pathRight;
                }
                else
                {
                    rci.rightTop = rand.Next(0, REGION_HEIGHT / 4);
                    rci.rightBottom = rand.Next(REGION_HEIGHT * 3 / 4, REGION_HEIGHT);
                    rci.pathRight = rand.Next(REGION_HEIGHT / 3, REGION_HEIGHT * 2 / 3);
                }
            }

            return rcis;
        }

        /// <summary>
        /// Adds a fractal to a region
        /// </summary>
        /// <param name="rci"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void createFractal(RegionConstructionInfo rci, Point start, Point end)
        {
            float maxOffset = 60.0f;
            float threshold = 7000.0f;
            
            List<LineSegment> fractal = 
                Fractal.fractalLineSegment(
                    new LineSegment(new Vector2(start.X, start.Y), new Vector2(end.X, end.Y)),
                    maxOffset,
                    threshold);

            rci.fractals.Add(new LineFractalInfo(fractal));
        }

        /// <summary>
        /// Adds all fractals to a region based on which sides it has openings to other regions
        /// </summary>
        /// <param name="rcis"></param>
        public static void createFractals(Dictionary<Point, RegionConstructionInfo> rcis)
        {
            int top = -1;
            int bottom = REGION_HEIGHT + 1;// -1;
            int left = -1;
            int right = REGION_WIDTH + 1;// -1;

            foreach (RegionConstructionInfo rci in rcis.Values)
            {
                // first identify the case
                int sides = 0;
                if (rci.marker.connectsTop) sides++;
                if (rci.marker.connectsLeft) sides++;
                if (rci.marker.connectsBottom) sides++;
                if (rci.marker.connectsRight) sides++;
                bool topAndBottom = rci.marker.connectsTop && rci.marker.connectsBottom;
                bool leftAndRight = rci.marker.connectsLeft && rci.marker.connectsRight;

                // dead ends
                if (sides == 1)
                {
                    if (rci.marker.connectsTop)
                    {
                        createFractal(rci, new Point(rci.topRight, top), new Point(rci.bottomRight, rci.rightBottom));
                        createFractal(rci, new Point(rci.bottomRight, rci.rightBottom), new Point(rci.bottomLeft, rci.leftBottom));
                        createFractal(rci, new Point(rci.bottomLeft, rci.leftBottom), new Point(rci.topLeft, top));
                    }
                    else if (rci.marker.connectsLeft)
                    {
                        createFractal(rci, new Point(left, rci.leftTop), new Point(rci.topRight, rci.rightTop));
                        createFractal(rci, new Point(rci.topRight, rci.rightTop), new Point(rci.bottomRight, rci.rightBottom));
                        createFractal(rci, new Point(rci.bottomRight, rci.rightBottom), new Point(left, rci.leftBottom));
                    }
                    else if (rci.marker.connectsBottom)
                    {
                        createFractal(rci, new Point(rci.bottomLeft, bottom), new Point(rci.topLeft, rci.leftTop));
                        createFractal(rci, new Point(rci.topLeft, rci.leftTop), new Point(rci.topRight, rci.rightTop));
                        createFractal(rci, new Point(rci.topRight, rci.rightTop), new Point(rci.bottomRight, bottom));
                    }
                    else if (rci.marker.connectsRight)
                    {
                        createFractal(rci, new Point(right, rci.rightBottom), new Point(rci.bottomLeft, rci.leftBottom));
                        createFractal(rci, new Point(rci.bottomLeft, rci.leftBottom), new Point(rci.topLeft, rci.leftTop));
                        createFractal(rci, new Point(rci.topLeft, rci.leftTop), new Point(right, rci.rightTop));
                    }
                }

                // corners
                if (sides == 2 && !topAndBottom && !leftAndRight)
                {
                    if (rci.marker.connectsLeft && rci.marker.connectsTop)
                    {
                        // outer
                        createFractal(rci, new Point(rci.topRight, top), new Point(rci.bottomRight, rci.rightBottom));
                        createFractal(rci, new Point(rci.bottomRight, rci.rightBottom), new Point(left, rci.leftBottom));

                        // inner topleft
                        createFractal(rci, new Point(left, rci.leftTop), new Point(rci.topLeft, rci.leftTop));
                        createFractal(rci, new Point(rci.topLeft, rci.leftTop), new Point(rci.topLeft, top));
                    }
                    else if (rci.marker.connectsLeft && rci.marker.connectsBottom)
                    {
                        // outer
                        createFractal(rci, new Point(left, rci.leftTop), new Point(rci.topRight, rci.rightTop));
                        createFractal(rci, new Point(rci.topRight, rci.rightTop), new Point(rci.bottomRight, bottom));

                        // inner leftbottom
                        createFractal(rci, new Point(rci.bottomLeft, bottom), new Point(rci.bottomLeft, rci.leftBottom));
                        createFractal(rci, new Point(rci.bottomLeft, rci.leftBottom), new Point(left, rci.leftBottom));
                    }
                    else if (rci.marker.connectsRight && rci.marker.connectsTop)
                    {
                        // outer
                        createFractal(rci, new Point(right, rci.rightBottom), new Point(rci.bottomLeft, rci.leftBottom));
                        createFractal(rci, new Point(rci.bottomLeft, rci.leftBottom), new Point(rci.topLeft, top));

                        // inner topright
                        createFractal(rci, new Point(rci.topRight, top), new Point(rci.topRight, rci.rightTop));
                        createFractal(rci, new Point(rci.topRight, rci.rightTop), new Point(right, rci.rightTop));
                    }
                    else if (rci.marker.connectsRight && rci.marker.connectsBottom)
                    {
                        // outer
                        createFractal(rci, new Point(rci.bottomLeft, bottom), new Point(rci.topLeft, rci.leftTop));
                        createFractal(rci, new Point(rci.topLeft, rci.leftTop), new Point(right, rci.rightTop));

                        // inner bottomright
                        createFractal(rci, new Point(right, rci.rightBottom), new Point(rci.bottomRight, rci.rightBottom));
                        createFractal(rci, new Point(rci.bottomRight, rci.rightBottom), new Point(rci.bottomRight, bottom));
                    }
                }

                // vertical
                if (sides == 2 && topAndBottom)
                {
                    // left line
                    createFractal(rci, new Point(rci.bottomLeft, bottom), new Point(rci.topLeft, top));

                    // right line
                    createFractal(rci, new Point(rci.topRight, top), new Point(rci.bottomRight, bottom));
                }

                // horizontal
                if (sides == 2 && leftAndRight)
                {
                    // top line
                    createFractal(rci, new Point(left, rci.leftTop), new Point(right, rci.rightTop));

                    // bottom line
                    createFractal(rci, new Point(right, rci.rightBottom), new Point(left, rci.leftBottom));
                }

                // t-intersections
                if (sides == 3)
                {
                    if (topAndBottom && rci.marker.connectsLeft)
                    {
                        // right line
                        createFractal(rci, new Point(rci.topRight, top), new Point(rci.bottomRight, bottom));

                        // inner topleft
                        createFractal(rci, new Point(left, rci.leftTop), new Point(rci.topLeft, rci.leftTop));
                        createFractal(rci, new Point(rci.topLeft, rci.leftTop), new Point(rci.topLeft, top));

                        // inner leftbottom
                        createFractal(rci, new Point(rci.bottomLeft, bottom), new Point(rci.bottomLeft, rci.leftBottom));
                        createFractal(rci, new Point(rci.bottomLeft, rci.leftBottom), new Point(left, rci.leftBottom));
                    }
                    else if (topAndBottom && rci.marker.connectsRight)
                    {
                        // left line
                        createFractal(rci, new Point(rci.bottomLeft, bottom), new Point(rci.topLeft, top));

                        // inner topright
                        createFractal(rci, new Point(rci.topRight, top), new Point(rci.topRight, rci.rightTop));
                        createFractal(rci, new Point(rci.topRight, rci.rightTop), new Point(right, rci.rightTop));

                        // inner bottomright
                        createFractal(rci, new Point(right, rci.rightBottom), new Point(rci.bottomRight, rci.rightBottom));
                        createFractal(rci, new Point(rci.bottomRight, rci.rightBottom), new Point(rci.bottomRight, bottom));
                    }
                    else if (leftAndRight && rci.marker.connectsTop)
                    {
                        // bottom line
                        createFractal(rci, new Point(right, rci.rightBottom), new Point(left, rci.leftBottom));

                        // inner topright
                        createFractal(rci, new Point(rci.topRight, top), new Point(rci.topRight, rci.rightTop));
                        createFractal(rci, new Point(rci.topRight, rci.rightTop), new Point(right, rci.rightTop));

                        // inner topleft
                        createFractal(rci, new Point(left, rci.leftTop), new Point(rci.topLeft, rci.leftTop));
                        createFractal(rci, new Point(rci.topLeft, rci.leftTop), new Point(rci.topLeft, top));
                    }
                    else if (leftAndRight && rci.marker.connectsBottom)
                    {
                        // top line
                        createFractal(rci, new Point(left, rci.leftTop), new Point(right, rci.rightTop));

                        // inner leftbottom
                        createFractal(rci, new Point(rci.bottomLeft, bottom), new Point(rci.bottomLeft, rci.leftBottom));
                        createFractal(rci, new Point(rci.bottomLeft, rci.leftBottom), new Point(left, rci.leftBottom));

                        // inner bottomright
                        createFractal(rci, new Point(right, rci.rightBottom), new Point(rci.bottomRight, rci.rightBottom));
                        createFractal(rci, new Point(rci.bottomRight, rci.rightBottom), new Point(rci.bottomRight, bottom));
                    }
                }

                // openness
                if (sides == 4)
                {
                    // inner topright
                    createFractal(rci, new Point(rci.topRight, top), new Point(rci.topRight, rci.rightTop));
                    createFractal(rci, new Point(rci.topRight, rci.rightTop), new Point(right, rci.rightTop));

                    // inner topleft
                    createFractal(rci, new Point(rci.topLeft, top), new Point(rci.topLeft, rci.leftTop));
                    createFractal(rci, new Point(rci.topLeft, rci.leftTop), new Point(left, rci.leftTop));

                    // inner leftbottom
                    createFractal(rci, new Point(rci.bottomLeft, bottom), new Point(rci.bottomLeft, rci.leftBottom));
                    createFractal(rci, new Point(rci.bottomLeft, rci.leftBottom), new Point(left, rci.leftBottom));

                    // inner bottomright
                    createFractal(rci, new Point(rci.bottomRight, bottom), new Point(rci.bottomRight, rci.rightBottom));
                    createFractal(rci, new Point(rci.bottomRight, rci.rightBottom), new Point(right, rci.rightBottom));
                }
            }
        }

        /// <summary>
        /// Instantiates regions from RegionConstructionInfo's by checking their shapes and adding appropriate
        /// boundary fractals
        /// </summary>
        /// <param name="rcis"></param>
        /// <returns></returns>
        public static Dictionary<Point,Region> createRegions(Dictionary<Point, RegionConstructionInfo> rcis)
        {
            Dictionary<Point, Region> regions = new Dictionary<Point, Region>();
            foreach (RegionConstructionInfo rci in rcis.Values)
            {
                //Environment env = Environment.construct(rci.marker.environment);
                Region region = constructRegion(rci);

                regions.Add(region.location, region);
            }

            return regions;
        }

        #region Region Creation

        /// <summary>
        /// Creates a Region based on the specified parameters.
        /// </summary>
        /// <param name="rci">Properties of the region to be created.</param>
        /// <returns>A Region object with its areas all created and populated appropriately.</returns>
        public static Region constructRegion(RegionConstructionInfo rci)
        {
            Region r = new Region(rci.location);

            for (int i = 0; i < Region.AREAS_WIDE; ++i)
            {
                for (int j = 0; j < Region.AREAS_TALL; ++j)
                {
                    Point globalLocation = new Point(
                        rci.location.X * Region.AREAS_WIDE + i,
                        rci.location.Y * Region.AREAS_TALL + j);
                    Area a = Area.makeEmptyArea(globalLocation);
                    a.fractals = mapFractals(rci.fractals, i, j);

                    populateArea(a);

                    r.areas[i, j] = a;
                }
            }

            return r;
        }

        /// <summary>
        /// Maps fractals at the Region level into fractals at the Area level.
        /// </summary>
        /// <param name="fractals">Fractals to be mapped.</param>
        /// <param name="areaX">X position of Area RELATIVE TO THE REGION.</param>
        /// <param name="areaY">Y position of Area RELATIVE TO THE REGION.</param>
        /// <returns>Fractals with coordinates corresponding to the specified Area's coordinate system.</returns>
        public static List<LineFractalInfo> mapFractals(List<LineFractalInfo> fractals, int areaX, int areaY)
        {
            List<LineFractalInfo> mappedFractals = new List<LineFractalInfo>();
            foreach (LineFractalInfo lfi in fractals)
            {
                List<LineSegment> mappedFractal = new List<LineSegment>();
                foreach (LineSegment ls in lfi.fractal)
                {
                    LineSegment mappedSeg = new LineSegment(
                        Region.mapCoordToArea(ls.p, areaX, areaY),
                        Region.mapCoordToArea(ls.q, areaX, areaY));
                    mappedFractal.Add(mappedSeg);
                }
                mappedFractals.Add(new LineFractalInfo(mappedFractal));
            }

            return mappedFractals;
        }

        #endregion

        #region Area Population

        public class EnvironmentFillInfo
        {
            public int EMPTY;
            public int FILL;
            public int FILL_LEFT;
            public int FILL_RIGHT;
            public int FILL_TOP;
            public int FILL_BOTTOM;
            public int FILL_TOPRIGHT;
            public int FILL_TOPLEFT;
            public int FILL_BOTTOMRIGHT;
            public int FILL_BOTTOMLEFT;
            public int EMPTY_TOPRIGHT;
            public int EMPTY_TOPLEFT;
            public int EMPTY_BOTTOMRIGHT;
            public int EMPTY_BOTTOMLEFT;
            public int OTHER;
        }

        public static EnvironmentFillInfo getCrateriaEnvironmentFillInfo()
        {
            EnvironmentFillInfo efi = new EnvironmentFillInfo();
            efi.EMPTY = 3;
            efi.EMPTY_BOTTOMLEFT = 13;
            efi.EMPTY_BOTTOMRIGHT = 14;
            efi.EMPTY_TOPLEFT = 8;
            efi.EMPTY_TOPRIGHT = 9;
            efi.FILL = 6;
            efi.FILL_BOTTOM = 1;
            efi.FILL_BOTTOMLEFT = 2;
            efi.FILL_BOTTOMRIGHT = 0;
            efi.FILL_LEFT = 7;
            efi.FILL_RIGHT = 5;
            efi.FILL_TOP = 11;
            efi.FILL_TOPLEFT = 12;
            efi.FILL_TOPRIGHT = 10;
            efi.OTHER = 4;

            return efi;
        }

        public static void populateArea(Area a)
        {
            EnvironmentFillInfo efi = getCrateriaEnvironmentFillInfo(); // TODO data-drive this bad boy

            Dictionary<Point, GridUtils.TileConfigurer> tileConfigs =
                new Dictionary<Point, GridUtils.TileConfigurer>();

            Dictionary<Point, bool> tilesToFix = new Dictionary<Point, bool>();
            //Dictionary<Point, bool> fixedTiles = new Dictionary<Point,bool>();

            // mark all cells hit by the fractals, and which sides they are hit on, using
            //  grid intersection tests
            HashSet<Point> markedCellsSet = new HashSet<Point>(); // prevents duplicates
            // important because adding another fractal may change our interpretation
            //  of a tile, so we do all fractals first then figure out the tiles
            foreach (LineFractalInfo lfi in a.fractals)
            {
                // mark cells hit by fractal
                List<Point> markedCells = GridUtils.getGridIntersects(lfi.fractal, 40, 40, tileConfigs);

                // and add them all to a total set of hit cells
                foreach (Point p in markedCells)
                {
                    markedCellsSet.Add(p);
                }
            }

            // now that we know where all cells have been hit and where, we can figure out
            //  what type of tile they are, and also flag their neighbors for correction if
            //  necessary
            foreach (Point p in markedCellsSet)
            {
                if (p.X <= a.Tiles.GetUpperBound(0) &&
                    p.Y <= a.Tiles.GetUpperBound(1) &&
                    p.X >= 0 &&
                    p.Y >= 0)
                {
                    switch (tileConfigs[p].getType())
                    {
                        case GridUtils.TILE_TYPE.EMPTY:
                            a.Tiles[p.X, p.Y] = efi.EMPTY;
                            break;
                        case GridUtils.TILE_TYPE.FILL:
                            a.Tiles[p.X, p.Y] = efi.FILL;
                            break;
                        case GridUtils.TILE_TYPE.FILL_LEFT:
                            a.Tiles[p.X, p.Y] = efi.FILL_LEFT;
                            tilesToFix[new Point(p.X - 1, p.Y)] = false;
                            break;
                        case GridUtils.TILE_TYPE.FILL_RIGHT:
                            a.Tiles[p.X, p.Y] = efi.FILL_RIGHT;
                            tilesToFix[new Point(p.X + 1, p.Y)] = false;
                            break;
                        case GridUtils.TILE_TYPE.FILL_TOP:
                            a.Tiles[p.X, p.Y] = efi.FILL_TOP;
                            tilesToFix[new Point(p.X, p.Y - 1)] = false;
                            break;
                        case GridUtils.TILE_TYPE.FILL_BOTTOM:
                            a.Tiles[p.X, p.Y] = efi.FILL_BOTTOM;
                            tilesToFix[new Point(p.X, p.Y + 1)] = false;
                            break;
                        case GridUtils.TILE_TYPE.FILL_TOPLEFT:
                            a.Tiles[p.X, p.Y] = efi.FILL_TOPLEFT;
                            break;
                        case GridUtils.TILE_TYPE.FILL_TOPRIGHT:
                            a.Tiles[p.X, p.Y] = efi.FILL_TOPRIGHT;
                            break;
                        case GridUtils.TILE_TYPE.FILL_BOTTOMLEFT:
                            a.Tiles[p.X, p.Y] = efi.FILL_BOTTOMLEFT;
                            break;
                        case GridUtils.TILE_TYPE.FILL_BOTTOMRIGHT:
                            a.Tiles[p.X, p.Y] = efi.FILL_BOTTOMRIGHT;
                            break;
                        case GridUtils.TILE_TYPE.EMPTY_TOPLEFT:
                            a.Tiles[p.X, p.Y] = efi.EMPTY_TOPLEFT;
                            break;
                        case GridUtils.TILE_TYPE.EMPTY_TOPRIGHT:
                            a.Tiles[p.X, p.Y] = efi.EMPTY_TOPRIGHT;
                            break;
                        case GridUtils.TILE_TYPE.EMPTY_BOTTOMLEFT:
                            a.Tiles[p.X, p.Y] = efi.EMPTY_BOTTOMLEFT;
                            break;
                        case GridUtils.TILE_TYPE.EMPTY_BOTTOMRIGHT:
                            a.Tiles[p.X, p.Y] = efi.EMPTY_BOTTOMRIGHT;
                            break;
                        default:
                            a.Tiles[p.X, p.Y] = efi.OTHER;
                            break;
                    }

                    // if not empty, mark that we've set it - if empty it probably means
                    //  a tile which a fractal double backed on, so we don't want to make
                    //  any guarantees
                    if (tileConfigs[p].getType() != GridUtils.TILE_TYPE.EMPTY)
                    {
                        tilesToFix[p] = true;
                    }
                }
            }

            // copy list of tiles to fix into an array since we modify it
            //  while we are traversing it, then use recursion to flood the changes
            Point[] points = new Point[tilesToFix.Count];
            tilesToFix.Keys.CopyTo(points, 0);
            foreach (Point p in points)
            {
                fill(p, tilesToFix, a.Tiles, efi);
            }

            foreach (Point p in tilesToFix.Keys)
            {
                a.FixedTiles.Add(p);
            }
            a.initializeTileColliders();
        }

        public static void fill(Point p, Dictionary<Point, bool> tilesToFix, int[,] tiles, EnvironmentFillInfo efi)
        {
            if (tilesToFix.ContainsKey(p) && tilesToFix[p] == true)
            {
                return;
            }
            if (p.X < 0 || p.Y < 0 || p.X >= Area.WIDTH_IN_TILES || p.Y >= Area.HEIGHT_IN_TILES)
            {
                return;
            }
            tiles[p.X, p.Y] = efi.FILL;
            tilesToFix[p] = true;
            fill(new Point(p.X - 1, p.Y), tilesToFix, tiles, efi);
            fill(new Point(p.X + 1, p.Y), tilesToFix, tiles, efi);
            fill(new Point(p.X, p.Y - 1), tilesToFix, tiles, efi);
            fill(new Point(p.X, p.Y + 1), tilesToFix, tiles, efi);
        }

        #endregion

    }
}
