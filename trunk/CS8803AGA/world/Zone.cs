using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidAIGameLibrary;
using MetroidAI.collision;
using MetroidAI.controllers;
using Microsoft.Xna.Framework.Graphics;
using MetroidAI.world.space;
using MetroidAI.engine;
using MetroidAI.world.space.postprocessors;

namespace MetroidAI.world
{
    public class Zone
    {
        #region Fields and Properties

        private class ZoneInitializationLock {}

        static GameFont s_font = FontMap.getInstance().getFont(FontEnum.Kootenay8);

        public const int TILE_WIDTH = 40;
        public const int TILE_HEIGHT = 40;

        public const int SCREEN_WIDTH_IN_TILES = 34;
        public const int SCREEN_HEIGHT_IN_TILES = 18;

        public const int SCREEN_WIDTH_IN_PIXELS = TILE_WIDTH * SCREEN_WIDTH_IN_TILES;
        public const int SCREEN_HEIGHT_IN_PIXELS = TILE_HEIGHT * SCREEN_HEIGHT_IN_TILES;

        public Dictionary<Point, ScreenConstructionInfo> ScreenConstructionInfos { get; protected set; }

        public Point TopLeftPosition { get; protected set; }
        public List<Point> PositionsOwned { get; protected set; }

        public bool HasBeenVisited { get; set; }

        public TileSet TileSet
        { get; protected set; }

        public Dictionary<Point, int[,]> Tiles
        { get; protected set; }

        public GameTexture TilesetTexture
        { get; protected set; }

        public CollisionDetector CollisionDetector
        { get; protected set; }

        public List<IGameObject> GameObjects
        { get; protected set; }

        public HashSet<KeyValuePair<Point,Point>> BlockedTiles
        {
            get
            {
                return m_blockedTiles;
            }
        }

        public HashSet<KeyValuePair<Point, Point>> FakeTiles { get; protected set; }

        public new Dictionary<Point, Dictionary<Point,String>> TileDebugLabels { get; protected set; }

        public bool IsInitialized { get; protected set; }

        internal MetroidAI.world.ZoneBuilder.EnvironmentFillInfo EnvironmentFillInfo { get { return m_efi; } }

        protected HashSet<KeyValuePair<Point, Point>> m_blockedTiles;

        // TODO store this information in the tile set...
        protected MetroidAI.world.ZoneBuilder.EnvironmentFillInfo m_efi;

        //internal Dictionary<Point, List<LineFractalInfo>> fractals;

        #endregion

        #region Construction & Initialization

        /// <summary>
        /// Load a TileSetInfo from content and create a Zone using it at the specified global coordinates
        /// </summary>
        /// <param name="tileSetPath"></param>
        /// <param name="location"></param>
        private Zone(string tileSetPath, List<ScreenConstructionInfo> screens)
        {
            // Initialize basic and empty properties

            this.m_blockedTiles = new HashSet<KeyValuePair<Point, Point>>();
            this.FakeTiles = new HashSet<KeyValuePair<Point, Point>>();
            this.TileDebugLabels = new Dictionary<Point, Dictionary<Point, String>>();

            this.CollisionDetector = new CollisionDetector();
            this.GameObjects = new List<IGameObject>();

            this.m_efi = ZoneBuilder.getCrateriaEnvironmentFillInfo();

            // Initialize properties from ScreenConstructionInfos

            this.ScreenConstructionInfos = new Dictionary<Point, ScreenConstructionInfo>();
            this.PositionsOwned = new List<Point>();
            this.Tiles = new Dictionary<Point, int[,]>();

            screens.ForEach(i => {
                this.PositionsOwned.Add(i.Location);
                this.ScreenConstructionInfos.Add(i.Location, i);
                this.Tiles[i.Location] = new int[SCREEN_WIDTH_IN_TILES, SCREEN_HEIGHT_IN_TILES];

                this.TileDebugLabels[i.Location] = new Dictionary<Point, string>();
            });

            // Find upper left most screen

            Point upperLeft = new Point(Int32.MaxValue, Int32.MaxValue);
            foreach (ScreenConstructionInfo pt in screens)
            {
                upperLeft.X = Math.Min(pt.Location.X, upperLeft.X);
                upperLeft.Y = Math.Min(pt.Location.Y, upperLeft.Y);
            }
            this.TopLeftPosition = upperLeft;

            // Initialize graphics and tileset

            this.TileSet = GlobalHelper.loadContent<TileSet>(tileSetPath);
            this.TilesetTexture = new GameTexture(TileSet.assetPath, TileSet.getSpriteSheetSourceRectangles());

            // And register this zone with the world!

            WorldManager.RegisterZone(this);
        }

        /// <summary>
        /// Creates an empty zone using the provided screen construction infos
        /// </summary>
        /// <returns></returns>
        public static Zone makeEmptyZone(List<ScreenConstructionInfo> screens)
        {
            return makeEmptyZone(@"Sprites/Crateria", screens);
        }

        /// <summary>
        /// Creates an empty zone using a specified tileset using provided screen construction infos
        /// </summary>
        /// <param name="tileSetpath"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static Zone makeEmptyZone(string tileSetpath, List<ScreenConstructionInfo> screens)
        {
            Zone z = new Zone(tileSetpath, screens);

            // default area
            foreach (Point p in z.PositionsOwned)
            {
                for (int i = 0; i < SCREEN_WIDTH_IN_TILES; ++i)
                    for (int j = 0; j < SCREEN_HEIGHT_IN_TILES; ++j)
                    {
                        z.Tiles[p][i, j] = z.m_efi.EMPTY;
                    }
            }

            return z;
        }

        /// <summary>
        /// Uses fractals to tile the zone, then verifies no major problems occurred.
        /// </summary>
        public void Initialize()
        {
            // already initialized, don't bother grabbing lock
            if (IsInitialized) return;

            lock (this)
            {
                // check again in case thing which had lock before initialized this zone
                if (IsInitialized) return;

                foreach (Point globalScreenCoord in PositionsOwned)
                {
                    bool valid = false;
                    while (!valid)
                    {
                        // Fills the ScreenConstructionInfo with fractal information
                        this.CalculateFractals(globalScreenCoord, ScreenConstructionInfos[globalScreenCoord].Parameters.FractalCreator);
                        
                        // Use fractals to create initial tiles
                        this.PopulateTiles(globalScreenCoord);

                        // Guarantee a path - post processing can remove it, if desired
                        this.ClearAPath(globalScreenCoord); // ignore return value because we're checking all tiles anyway

                        // Post processing for screen-specific information
                        // Here we can make it so only ball can get through, hidden paths, etc.
                        this.PostProcess(globalScreenCoord);

                        // Fix invalid tiles - safest to just check all of them
                        this.FixInvalidTiles(globalScreenCoord, getFullCheckset());

                        // Determine if we had an error which accidentally filled the whole screen
                        valid = this.TestValidity(globalScreenCoord);

                        if (!valid)
                        {
                            Reset(globalScreenCoord);
                        }
                    }

                    // Enemies and other objects which depend on finalized tiling
                    this.PopulateObjects(globalScreenCoord);
                }

                initializeTileColliders();

                this.IsInitialized = true; // only flag as initialized once done
            }
        }

        protected bool TestValidity(Point globalScreenCoord)
        {
            int count = 0;
            for (int i = 0; i < SCREEN_WIDTH_IN_TILES; ++i)
            {
                for (int j = 0; j < SCREEN_HEIGHT_IN_TILES; ++j)
                {
                    if (TileSet.tileInfos[Tiles[globalScreenCoord][i, j]].passable)
                    {
                        count++;
                    }
                }
            }

            return (count > SCREEN_WIDTH_IN_TILES * SCREEN_HEIGHT_IN_TILES * 0.2f);
        }

        public void Reset(Point globalScreenCoord)
        {
            for (int i = 0; i < SCREEN_WIDTH_IN_TILES; ++i)
            {
                for (int j = 0; j < SCREEN_HEIGHT_IN_TILES; ++j)
                {
                    Tiles[globalScreenCoord][i, j] = m_efi.EMPTY;
                }
            }

            BlockedTiles.RemoveWhere(i => i.Key == globalScreenCoord);
            ScreenConstructionInfo sci = ScreenConstructionInfos[globalScreenCoord];
            sci.Fractals.Clear();
        }

        protected void CalculateFractals(Point globalScreenCoord, IFractalCreator fca)
        {
            fca.CreateFractals(ScreenConstructionInfos[globalScreenCoord]);
        }

        /// <summary>
        /// Places tiles in the zone based upon its fractals.
        /// </summary>
        public void PopulateTiles(Point screenPos)
        {
            Dictionary<Point, GridUtils.TileConfigurer> tileConfigs =
                new Dictionary<Point, GridUtils.TileConfigurer>();

            HashSet<Point> tilesToFix = new HashSet<Point>();
            HashSet<Point> fixedTiles = new HashSet<Point>();

            // mark all cells hit by the fractals, and which sides they are hit on, using
            //  grid intersection tests
            HashSet<Point> markedCellsSet = new HashSet<Point>(); // prevents duplicates
            // important because adding another fractal may change our interpretation
            //  of a tile, so we do all fractals first then figure out the tiles
            foreach (LineFractalInfo lfi in ScreenConstructionInfos[screenPos].Fractals)
            {
                // mark cells hit by fractal
                List<Point> markedCells =
                    GridUtils.getGridIntersects(lfi.fractal, Zone.TILE_WIDTH, Zone.TILE_HEIGHT, tileConfigs);

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
                if (p.X < SCREEN_WIDTH_IN_TILES &&
                    p.Y < SCREEN_HEIGHT_IN_TILES &&
                    p.X >= 0 &&
                    p.Y >= 0)
                {
                    switch (tileConfigs[p].getType())
                    {
                        case GridUtils.TILE_TYPE.EMPTY:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.EMPTY;
                            break;
                        case GridUtils.TILE_TYPE.FILL:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.FILL;
                            break;
                        case GridUtils.TILE_TYPE.FILL_LEFT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.FILL_LEFT;
                            tilesToFix.Add(new Point(p.X - 1, p.Y));
                            break;
                        case GridUtils.TILE_TYPE.FILL_RIGHT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.FILL_RIGHT;
                            tilesToFix.Add(new Point(p.X + 1, p.Y));
                            break;
                        case GridUtils.TILE_TYPE.FILL_TOP:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.FILL_TOP;
                            tilesToFix.Add(new Point(p.X, p.Y - 1));
                            break;
                        case GridUtils.TILE_TYPE.FILL_BOTTOM:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.FILL_BOTTOM;
                            tilesToFix.Add(new Point(p.X, p.Y + 1));
                            break;
                        case GridUtils.TILE_TYPE.FILL_TOPLEFT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.FILL_TOPLEFT;
                            break;
                        case GridUtils.TILE_TYPE.FILL_TOPRIGHT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.FILL_TOPRIGHT;
                            break;
                        case GridUtils.TILE_TYPE.FILL_BOTTOMLEFT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.FILL_BOTTOMLEFT;
                            break;
                        case GridUtils.TILE_TYPE.FILL_BOTTOMRIGHT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.FILL_BOTTOMRIGHT;
                            break;
                        case GridUtils.TILE_TYPE.EMPTY_TOPLEFT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.EMPTY_TOPLEFT;
                            break;
                        case GridUtils.TILE_TYPE.EMPTY_TOPRIGHT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.EMPTY_TOPRIGHT;
                            break;
                        case GridUtils.TILE_TYPE.EMPTY_BOTTOMLEFT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.EMPTY_BOTTOMLEFT;
                            break;
                        case GridUtils.TILE_TYPE.EMPTY_BOTTOMRIGHT:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.EMPTY_BOTTOMRIGHT;
                            break;
                        default:
                            this.Tiles[screenPos][p.X, p.Y] = m_efi.OTHER;
                            break;
                    }

                    // if not empty, mark that we've set it - if empty it probably means
                    //  a tile which a fractal double backed on, so we don't want to make
                    //  any guarantees
                    if (tileConfigs[p].getType() != GridUtils.TILE_TYPE.EMPTY)
                    {
                        fixedTiles.Add(p);
                    }
                }
            }

            // copy list of tiles to fix into an array since we modify it
            //  while we are traversing it, then use recursion to flood the changes
            //Point[] points = new Point[tilesToFix.Count];
            //tilesToFix.Keys.CopyTo(points, 0);
            foreach (Point p in tilesToFix)
            {
                fill(p, fixedTiles, tilesToFix, this.Tiles[screenPos]);
            }

            foreach (Point p in fixedTiles)
            {
                BlockedTiles.Add(new KeyValuePair<Point, Point>(screenPos, p));
            }
        }

        /// <summary>
        /// Clears a width-3 path through a screen based on the screen's PathPts.
        /// </summary>
        /// <param name="globalScreenCoord">Screen in which a path is being cleared.</param>
        /// <returns>A set of all points which should be rechecked for validity.</returns>
        protected HashSet<Point> ClearAPath(Point globalScreenCoord)
        {
            HashSet<Point> pointsToCorrect = new HashSet<Point>();

            ScreenConstructionInfo sci = ScreenConstructionInfos[globalScreenCoord];
            
            foreach (Direction d in Direction.All)
            {
                if (!sci.Parameters.Connections.ContainsKey(d) ||
                    sci.Parameters.Connections[d] == Connection.None)
                {
                    continue;
                }

                int primaryCoord = sci.OutsidePts[d];
                int secondaryCoord = sci.PathPts[d];
                if (d.IsVertical) ZoneBuilder.swap(ref primaryCoord, ref secondaryCoord);
                Vector2 start = new Vector2(primaryCoord, secondaryCoord);

                primaryCoord = (sci.OutsidePts[d] - sci.OutsidePts[d.Opposite])/2 + sci.OutsidePts[d.Opposite];
                secondaryCoord = (sci.PathPts[d] - sci.PathPts[d.Opposite])/2 + sci.PathPts[d.Opposite];
                if (d.IsVertical) ZoneBuilder.swap(ref primaryCoord, ref secondaryCoord);
                Vector2 end = new Vector2(primaryCoord, secondaryCoord);

                List<LineSegment> path = new List<LineSegment>();
                path.Add(new LineSegment(start, end));
                List<Point> intersectedTiles = GridUtils.getGridIntersects(
                    path,
                    TILE_WIDTH,
                    TILE_HEIGHT,
                    new Dictionary<Point, GridUtils.TileConfigurer>());

                foreach (Point p in intersectedTiles)
                {
                    Point cw = d.RotationCW.Move(p);
                    Point ccw = d.RotationCCW.Move(p);

                    BlockedTiles.Remove(new KeyValuePair<Point, Point>(globalScreenCoord, p));
                    BlockedTiles.Remove(new KeyValuePair<Point, Point>(globalScreenCoord, cw));
                    BlockedTiles.Remove(new KeyValuePair<Point, Point>(globalScreenCoord, ccw));

                    if (IsInScreen(p))
                    {
                        Tiles[globalScreenCoord][p.X, p.Y] = m_efi.EMPTY;
                        Tiles[globalScreenCoord][cw.X, cw.Y] = m_efi.EMPTY;
                        Tiles[globalScreenCoord][ccw.X, ccw.Y] = m_efi.EMPTY;

                        pointsToCorrect.Add(d.RotationCW.Move(cw));
                        pointsToCorrect.Add(d.RotationCCW.Move(ccw));
                    }
                }
            }

            return pointsToCorrect;
        }

        protected HashSet<Point> getFullCheckset()
        {
            HashSet<Point> checkSet = new HashSet<Point>();

            for (int i = 0; i < SCREEN_WIDTH_IN_TILES; ++i)
            {
                for (int j = 0; j < SCREEN_HEIGHT_IN_TILES; ++j)
                {
                    Point pt = new Point(i, j);
                    checkSet.Add(pt);
                }
            }

            return checkSet;
        }

        protected void PostProcess(Point globalScreenCoord)
        {
            ScreenConstructionInfos[globalScreenCoord].Parameters.PostProcessor.PostProcess(
                this, globalScreenCoord);
        }

        protected void FixInvalidTiles(Point globalScreenCoord, HashSet<Point> checkSet)
        {
            // maintain two sets; works like double buffer and reduces overall number
            // of times that we repeat checking certain points for invalidity.
            // ...yes, this assertion has yet to be verified.
            HashSet<Point> recheckSet = new HashSet<Point>();

            while (checkSet.Count > 0)
            {
                Point pt = checkSet.First();
                FixTile(globalScreenCoord, pt, recheckSet);
                checkSet.Remove(pt);

                if (checkSet.Count == 0)
                {
                    CommonFunctions.swap<HashSet<Point>>(ref checkSet, ref recheckSet);
                }
            }
        }

        void FixTile(Point globalScreenCoord, Point pt, HashSet<Point> checkSet)
        {
            KeyValuePair<Point, Point> ptPair = new KeyValuePair<Point, Point>(globalScreenCoord, pt);

            // only try to fix tile if on screen and currently a blocking type
            if (IsInScreen(pt) && BlockedTiles.Contains(ptPair))
            {
                // if it's not valid as blocking, we make it empty
                if (IsInvalid(globalScreenCoord, pt))
                {
                    BlockedTiles.Remove(ptPair);
                    checkSet.UnionWith(getNeighbors(pt));

                    Tiles[globalScreenCoord][pt.X, pt.Y] = m_efi.EMPTY;
                }
                else // otherwise it is a valid blocking type, so figure out what
                {
                    Tiles[globalScreenCoord][pt.X, pt.Y] = calculateTileType(globalScreenCoord, pt);
                }
            }
        }

        int calculateTileType(Point globalScreenCoord, Point tile)
        {
            Dictionary<Direction, bool> straightNeighborsBlocked =
                getStraightNeighborsBlocked(globalScreenCoord, tile);
            Dictionary<KeyValuePair<Direction, Direction>, bool> diagNeighborsBlocked =
                getDiagNeighborsBlocked(globalScreenCoord, tile);

            // if all straight sides filled, must be a fill tile or inner corner
            if (straightNeighborsBlocked[Direction.Up] &&
                straightNeighborsBlocked[Direction.Right] &&
                straightNeighborsBlocked[Direction.Left] &&
                straightNeighborsBlocked[Direction.Down])
            {
                if (!diagNeighborsBlocked[new KeyValuePair<Direction,Direction>(Direction.Up,Direction.Right)])
                    return m_efi.EMPTY_TOPRIGHT;
                if (!diagNeighborsBlocked[new KeyValuePair<Direction, Direction>(Direction.Right, Direction.Down)])
                    return m_efi.EMPTY_BOTTOMRIGHT;
                if (!diagNeighborsBlocked[new KeyValuePair<Direction, Direction>(Direction.Down, Direction.Left)])
                    return m_efi.EMPTY_BOTTOMLEFT;
                if (!diagNeighborsBlocked[new KeyValuePair<Direction, Direction>(Direction.Left, Direction.Up)])
                    return m_efi.EMPTY_TOPLEFT;
                return m_efi.FILL;
            }

            // edges
            if (!straightNeighborsBlocked[Direction.Up] &&
                straightNeighborsBlocked[Direction.Right] &&
                straightNeighborsBlocked[Direction.Down] &&
                straightNeighborsBlocked[Direction.Left])
            {
                return m_efi.FILL_BOTTOM;
            }
            if (straightNeighborsBlocked[Direction.Up] &&
                !straightNeighborsBlocked[Direction.Right] &&
                straightNeighborsBlocked[Direction.Down] &&
                straightNeighborsBlocked[Direction.Left])
            {
                return m_efi.FILL_LEFT;
            }
            if (straightNeighborsBlocked[Direction.Up] &&
                 straightNeighborsBlocked[Direction.Right] &&
                 !straightNeighborsBlocked[Direction.Down] &&
                 straightNeighborsBlocked[Direction.Left])
            {
                return m_efi.FILL_TOP;
            }
            if (straightNeighborsBlocked[Direction.Up] &&
                 straightNeighborsBlocked[Direction.Right] &&
                 straightNeighborsBlocked[Direction.Down] &&
                 !straightNeighborsBlocked[Direction.Left])
            {
                return m_efi.FILL_RIGHT;
            }

            // corners
            if (!straightNeighborsBlocked[Direction.Up] &&
                !straightNeighborsBlocked[Direction.Right])
            {
                return m_efi.FILL_BOTTOMLEFT;
            }
            if (!straightNeighborsBlocked[Direction.Right] &&
                !straightNeighborsBlocked[Direction.Down])
            {
                return m_efi.FILL_TOPLEFT;
            }
            if (!straightNeighborsBlocked[Direction.Down] &&
                !straightNeighborsBlocked[Direction.Left])
            {
                return m_efi.FILL_TOPRIGHT;
            }
            if (!straightNeighborsBlocked[Direction.Left] &&
                !straightNeighborsBlocked[Direction.Up])
            {
                return m_efi.FILL_BOTTOMRIGHT;
            }

            return m_efi.FILL;
        }

        HashSet<Point> getNeighbors(Point tile)
        {
            HashSet<Point> neighbors = new HashSet<Point>();

            foreach (Direction d in Direction.All)
            {
                neighbors.Add(d.Move(tile));
                neighbors.Add(d.Move(d.RotationCW.Move(tile)));
            }

            return neighbors;
        }

        Dictionary<Direction, bool> getStraightNeighborsBlocked
            (Point globalScreenCoord, Point tile)
        {
            Dictionary<Direction, bool> straightNeighborsBlocked =
                new Dictionary<Direction, bool>();

            foreach (Direction d in Direction.All)
            {
                Point neighbor = d.Move(tile);

                // if tile is a valid index, check it normally
                if (IsInScreen(neighbor))
                {
                    straightNeighborsBlocked.Add(
                        d,
                        BlockedTiles.Contains(new KeyValuePair<Point, Point>(globalScreenCoord, neighbor)));
                }
                else
                {
                    // tile is an invalid index - if the screen in that direction is also owned
                    // by this zone, we'll assume it's unblocked, otherwise we'll assume blocked
                    straightNeighborsBlocked.Add(
                        d,
                        !PositionsOwned.Contains(d.Move(globalScreenCoord)));
                }
            }

            return straightNeighborsBlocked;
        }

        Dictionary<KeyValuePair<Direction, Direction>, bool> getDiagNeighborsBlocked
            (Point globalScreenCoord, Point tile)
        {
            Dictionary<KeyValuePair<Direction,Direction>, bool> diagNeighborsBlocked =
                new Dictionary<KeyValuePair<Direction, Direction>, bool>();

            foreach (Direction d in Direction.All)
            {
                Point neighbor = d.Move(d.RotationCW.Move(tile));

                // if tile is a valid index, check it normally
                if (IsInScreen(neighbor))
                {
                    diagNeighborsBlocked.Add(
                        new KeyValuePair<Direction,Direction>(d, d.RotationCW),
                        BlockedTiles.Contains(new KeyValuePair<Point, Point>(globalScreenCoord, neighbor)));
                }
                else
                {
                    // tile is an invalid index - if the screen in that direction is also owned
                    // by this zone, we'll assume it's unblocked, otherwise we'll assume blocked
                    diagNeighborsBlocked.Add(
                        new KeyValuePair<Direction,Direction>(d, d.RotationCW),
                        !PositionsOwned.Contains(d.Move(d.RotationCW.Move(globalScreenCoord))));
                }
            }

            return diagNeighborsBlocked;
        }

        protected bool IsInvalid(Point globalScreenCoord, Point tile)
        {
            if (!IsInScreen(tile)) return false;

            Dictionary<Direction, bool> straightNeighborsBlocked =
                getStraightNeighborsBlocked(globalScreenCoord, tile);
            Dictionary<KeyValuePair<Direction, Direction>, bool> diagNeighborsBlocked =
                getDiagNeighborsBlocked(globalScreenCoord, tile);

            // detect opposites free
            if (!straightNeighborsBlocked[Direction.Up] &&
                !straightNeighborsBlocked[Direction.Down])
            {
                return true;
            }
            if (!straightNeighborsBlocked[Direction.Left] &&
                !straightNeighborsBlocked[Direction.Right])
            {
                return true;
            }

            // detect four corners
            if (!diagNeighborsBlocked[new KeyValuePair<Direction,Direction>(Direction.Up, Direction.Right)] &&
                !diagNeighborsBlocked[new KeyValuePair<Direction, Direction>(Direction.Right, Direction.Down)] &&
                !diagNeighborsBlocked[new KeyValuePair<Direction, Direction>(Direction.Down, Direction.Left)] &&
                !diagNeighborsBlocked[new KeyValuePair<Direction, Direction>(Direction.Left, Direction.Up)])
            {
                return true;
            }

            // detect triad
            foreach (Direction d in Direction.All)
            {
                if (!diagNeighborsBlocked[new KeyValuePair<Direction,Direction>(d, d.RotationCW)] &&
                    !straightNeighborsBlocked[d.Opposite] &&
                    !straightNeighborsBlocked[d.Opposite.RotationCW])
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsInScreen(Point tilePoint)
        {
            return !(tilePoint.X < 0 || tilePoint.Y < 0 ||
                    tilePoint.X >= SCREEN_WIDTH_IN_TILES ||
                    tilePoint.Y >= SCREEN_HEIGHT_IN_TILES);
        }

        protected void PopulateObjects(Point globalScreenCoord)
        {
            this.ScreenConstructionInfos[globalScreenCoord].Parameters.ObjectPopulator.PopulateObjects(this, globalScreenCoord);
        }

        /// <summary>
        /// Certain tile #s in a TileSet may be marked as impassable.  This function looks up those
        /// numbers and makes instances of those tiles in the Zone impassable by registering the
        /// appropriate collision box.
        /// </summary>
        public void initializeTileColliders()
        {
            foreach (Point globalScreenCoord in PositionsOwned)
            {
                Point zoneScreenCoord = getLocalScreenFromGlobalScreen(globalScreenCoord);

                for (int i = 0; i < SCREEN_WIDTH_IN_TILES; ++i)
                {
                    for (int j = 0; j < SCREEN_HEIGHT_IN_TILES; ++j)
                    {
                        if (!TileSet.tileInfos[Tiles[globalScreenCoord][i, j]].passable &&
                            !FakeTiles.Contains(new KeyValuePair<Point,Point>(globalScreenCoord, new Point(i,j))))
                        {
                            Rectangle bounds = getTileRectangle(zoneScreenCoord, i, j);
                            Collider ci = new Collider(null, bounds, ColliderType.Scenery);
                            CollisionDetector.register(ci);
                        }
                        else if (i == 0 && !this.PositionsOwned.Contains(new Point (globalScreenCoord.X-1, globalScreenCoord.Y)))
                        {
                            Rectangle bounds = getTileRectangle(zoneScreenCoord, i-1, j);
                            Collider ci = new Collider(null, bounds, ColliderType.Scenery);
                            CollisionDetector.register(ci);
                        }
                        else if (i == SCREEN_WIDTH_IN_TILES-1 && !this.PositionsOwned.Contains(new Point (globalScreenCoord.X+1, globalScreenCoord.Y)))
                        {
                            Rectangle bounds = getTileRectangle(zoneScreenCoord, i+1, j);
                            Collider ci = new Collider(null, bounds, ColliderType.Scenery);
                            CollisionDetector.register(ci);
                        }
                        else if (j == 0 && !this.PositionsOwned.Contains(new Point (globalScreenCoord.X, globalScreenCoord.Y-1)))
                        {
                            Rectangle bounds = getTileRectangle(zoneScreenCoord, i, j-1);
                            Collider ci = new Collider(null, bounds, ColliderType.Scenery);
                            CollisionDetector.register(ci);
                        }
                        else if (j == SCREEN_HEIGHT_IN_TILES-1 && !this.PositionsOwned.Contains(new Point (globalScreenCoord.X, globalScreenCoord.Y+1)))
                        {
                            Rectangle bounds = getTileRectangle(zoneScreenCoord, i, j+1);
                            Collider ci = new Collider(null, bounds, ColliderType.Scenery);
                            CollisionDetector.register(ci);
                        }
                    }
                }
            }
        }

        private void fill(Point p, HashSet<Point> fixedTiles, HashSet<Point> tilesToFix, int[,] tiles)
        {
            if (fixedTiles.Contains(p))
            {
                return;
            }
            if (p.X < 0 || p.Y < 0 || p.X >= Zone.SCREEN_WIDTH_IN_TILES || p.Y >= Zone.SCREEN_HEIGHT_IN_TILES)
            {
                return;
            }
            tiles[p.X, p.Y] = m_efi.FILL;
            fixedTiles.Add(p);
            fill(new Point(p.X - 1, p.Y), fixedTiles, tilesToFix, tiles);
            fill(new Point(p.X + 1, p.Y), fixedTiles, tilesToFix, tiles);
            fill(new Point(p.X, p.Y - 1), fixedTiles, tilesToFix, tiles);
            fill(new Point(p.X, p.Y + 1), fixedTiles, tilesToFix, tiles);
        }

        #endregion

        #region Helpers

        public Point getLocalScreenFromGlobalScreen(Point globalScreenCoord)
        {
            return new Point(globalScreenCoord.X - TopLeftPosition.X,
                             globalScreenCoord.Y - TopLeftPosition.Y);
        }

        public Point getGlobalScreenFromLocalScreen(Point localScreenCoord)
        {
            return new Point(localScreenCoord.X + TopLeftPosition.X,
                             localScreenCoord.Y + TopLeftPosition.Y);
        }

        public Point getLocalScreenOffsetInPixels(Point localScreenCoord)
        {
            return new Point(
                localScreenCoord.X * SCREEN_WIDTH_IN_PIXELS,
                localScreenCoord.Y * SCREEN_HEIGHT_IN_PIXELS);
        }

        public Vector2 getLocalScreenOffsetInPixelVector(Point localScreenCoord)
        {
            Point pt = getLocalScreenOffsetInPixels(localScreenCoord);
            return new Vector2(
                pt.X,
                pt.Y);
        }

        public Point getPixelPositionFromScreenPosition(Point zoneScreenCoord, Point pixelPosition)
        {
            Point offset = getLocalScreenOffsetInPixels(zoneScreenCoord);
            return new Point(
                offset.X + pixelPosition.X,
                offset.Y + pixelPosition.Y);
        }

        public Rectangle getScreenRectangle(Point zoneScreenCoord)
        {
            Point offset = getLocalScreenOffsetInPixels(zoneScreenCoord);
            return new Rectangle(
                offset.X,
                offset.Y,
                SCREEN_WIDTH_IN_PIXELS,
                SCREEN_HEIGHT_IN_PIXELS
                );
        }

        public Rectangle getTileRectangle(Point zoneScreenCoord, Point tile)
        {
            return getTileRectangle(zoneScreenCoord, tile.X, tile.Y);
        }

        public Rectangle getTileRectangle(Point zoneScreenCoord, int tileX, int tileY)
        {
            return new Rectangle(
                tileX * TileSet.tileWidth + SCREEN_WIDTH_IN_PIXELS * zoneScreenCoord.X,
                tileY * TileSet.tileHeight + SCREEN_HEIGHT_IN_PIXELS * zoneScreenCoord.Y,
                TileSet.tileWidth,
                TileSet.tileHeight);
        }

        public Point getLocalScreenCoordFromPosition(Vector2 position)
        {
            Point p = new Point((int)position.X / SCREEN_WIDTH_IN_PIXELS, (int)position.Y / SCREEN_HEIGHT_IN_PIXELS);
            if (position.X < 0) p.X -= 1;
            if (position.Y < 0) p.Y -= 1;
            return p;
        }

        public Point getGlobalScreenFromPosition(Vector2 position)
        {
            return getGlobalScreenFromLocalScreen(
                    getLocalScreenCoordFromPosition(position));
        }

        public bool getIsPositionInZone(Vector2 position)
        {
            Point zoneScreenCoord = getLocalScreenCoordFromPosition(position);
            Point globalScreenCoord = getGlobalScreenFromLocalScreen(zoneScreenCoord);
            return PositionsOwned.Contains(globalScreenCoord);
        }

        #endregion

        #region Add Items

        /// <summary>
        /// Place a GameObject into the zone
        /// </summary>
        /// <param name="gameObject"></param>
        public void add(IGameObject gameObject)
        {
            this.GameObjects.Add(gameObject);
        }

        /// <summary>
        /// Place a Collidable game object into the zone; handles registering collision box
        /// </summary>
        public void add(ICollidable collidable)
        {
            this.add(collidable, new Point(0, 0));
        }

        /// <summary>
        /// Place a Collidable game object into the zone; handles registering collision box
        /// </summary>
        public void add(ICollidable collidable, Point zoneScreenCoord)
        {
            collidable.getCollider().move(this.getLocalScreenOffsetInPixelVector(zoneScreenCoord));
            add((IGameObject)collidable);
            this.CollisionDetector.register(collidable.getCollider());
        }

        /// <summary>
        /// Remove a GameObject from the zone
        /// </summary>
        /// <param name="gameObject"></param>
        public void remove(IGameObject gameObject)
        {
            this.GameObjects.Remove(gameObject);
        }

        /// <summary>
        /// Remove a Collidable game object from the zone; handles registering collision box
        /// </summary>
        /// <param name="collidable"></param>
        public void remove(ICollidable collidable)
        {
            remove((IGameObject)collidable);
            this.CollisionDetector.remove(collidable.getCollider());
        }

        #endregion

        #region Draw Methods

        /// <summary>
        /// Normal draw method to be called during gameplay
        /// </summary>
        public void draw()
        {
            bool debugMode = Settings.getInstance().IsInDebugMode;

            // draw tiles

            foreach (Point pt in PositionsOwned)
            {
                Point screenOffset = getLocalScreenOffsetInPixels(getLocalScreenFromGlobalScreen(pt));

                for (int i = 0; i < SCREEN_WIDTH_IN_TILES; ++i)
                {
                    for (int j = 0; j < SCREEN_HEIGHT_IN_TILES; ++j)
                    {
                        Vector2 pos = new Vector2(
                            i * TileSet.tileWidth + screenOffset.X,
                            j * TileSet.tileHeight + screenOffset.Y);
                        float depth = Constants.DepthGameplayTiles;
                        // if we use equal depth for all tiles, they will all be rendered at once, meaning the graphics device
                        //  doesn't have to switch between sprites... much better performance

                        DrawCommand td = DrawBuffer.getInstance().DrawCommands.pushGet();
                        td.set(TilesetTexture, Tiles[pt][i, j], pos, CoordinateTypeEnum.RELATIVE, depth, false, Color.White, 0, 1.0f);
                    }
                }


            }

            // draw game objects

            foreach (IGameObject gameObject in GameObjects)
            {
                gameObject.draw();
            }

            // draw debug stuff

            if (debugMode)
            {
                // draw fractals

                foreach (Point pt in ScreenConstructionInfos.Keys)
                {
                    Point offset = getLocalScreenOffsetInPixels(getLocalScreenFromGlobalScreen(pt));

                    foreach (LineFractalInfo lfi in ScreenConstructionInfos[pt].Fractals)
                    {
                        List<LineSegment> fractal = lfi.fractal;
                        if (fractal != null)
                        {
                            foreach (LineSegment ls in fractal)
                            {
                                Vector2 start = new Vector2(ls.p.X + offset.X, ls.p.Y + offset.Y);
                                Vector2 end = new Vector2(ls.q.X + offset.X, ls.q.Y + offset.Y);
                                LineDrawer.drawLine(start, end, Color.BlueViolet);
                            }
                        }
                    }
                }

                // draw tile debug labels

                foreach (KeyValuePair<Point, Dictionary<Point, String>> kvpDict in TileDebugLabels)
                {
                    Point globalScreenCoord = kvpDict.Key;

                    Point screenOffset = getLocalScreenOffsetInPixels(
                                            getLocalScreenFromGlobalScreen(globalScreenCoord));

                    foreach (KeyValuePair<Point, String> kvp in kvpDict.Value)
                    {
                        Vector2 pos = new Vector2(
                            kvp.Key.X * TileSet.tileWidth + screenOffset.X + 5,
                            kvp.Key.Y * TileSet.tileHeight + screenOffset.Y + 5);

                        s_font.drawString(kvp.Value, pos, Color.Yellow, CoordinateTypeEnum.RELATIVE);
                    }
                }
            }
        }

        /// <summary>
        /// Scaled draw method for use in maps, minimaps, etc.
        /// </summary>
        /// <param name="offset">Pixel-position where the top left corner of the area should be drawn</param>
        /// <param name="scale">Amount the size of each graphic will be multiplied by when drawing</param>
        /// <param name="depth">z-Depth of the image</param>
        public void drawMap(Vector2 offset, float scale, float depth)
        {
            float screenSizeX = Zone.SCREEN_WIDTH_IN_PIXELS * scale;
            float screenSizeY = Zone.SCREEN_HEIGHT_IN_PIXELS * scale;

            int viewportWidth = EngineManager.Engine.GraphicsDevice.Viewport.Width;
            int viewportHeight = EngineManager.Engine.GraphicsDevice.Viewport.Height;

            foreach (Point pt in PositionsOwned)
            {
                if (pt.X < -screenSizeX || pt.X > viewportWidth + screenSizeX &&
                    pt.Y < -screenSizeY || pt.Y > viewportHeight + screenSizeY)
                {
                    continue;
                }

                Point screenOffset = getLocalScreenOffsetInPixels(getLocalScreenFromGlobalScreen(pt));

                // draw tiles
                for (int i = 0; i < SCREEN_WIDTH_IN_TILES; ++i)
                {
                    for (int j = 0; j < SCREEN_HEIGHT_IN_TILES; ++j)
                    {
                        Vector2 pos = new Vector2(
                            i * TileSet.tileWidth + screenOffset.X,
                            j * TileSet.tileHeight + screenOffset.Y);

                        DrawCommand td = DrawBuffer.getInstance().DrawCommands.pushGet();
                        td.set(TilesetTexture, Tiles[pt][i, j], pos * scale + offset, CoordinateTypeEnum.ABSOLUTE, depth, false, Color.White, 0, scale);
                    }
                }

                // draw box around screen
                Vector2 topLeft = new Vector2(screenOffset.X * scale + offset.X, screenOffset.Y * scale + offset.Y);
                Vector2 topRight = Direction.Right.Move(topLeft, SCREEN_WIDTH_IN_PIXELS * scale);
                Vector2 bottomLeft = Direction.Down.Move(topLeft, SCREEN_HEIGHT_IN_PIXELS * scale);
                Vector2 bottomRight = Direction.Right.Move(bottomLeft, SCREEN_WIDTH_IN_PIXELS * scale);
                if (PositionsOwned.Contains(Direction.Up.Move(pt)))
                {
                    LineDrawer.drawLine(topLeft, topRight, Color.Blue, CoordinateTypeEnum.ABSOLUTE);
                }
                else
                {
                    LineDrawer.drawLine(topLeft, topRight, Color.White, CoordinateTypeEnum.ABSOLUTE);
                }
                if (PositionsOwned.Contains(Direction.Left.Move(pt)))
                {
                    LineDrawer.drawLine(topLeft, bottomLeft, Color.Blue, CoordinateTypeEnum.ABSOLUTE);
                }
                else
                {
                    LineDrawer.drawLine(topLeft, bottomLeft, Color.White, CoordinateTypeEnum.ABSOLUTE);
                }
                if (PositionsOwned.Contains(Direction.Right.Move(pt)))
                {
                    LineDrawer.drawLine(topRight, bottomRight, Color.Blue, CoordinateTypeEnum.ABSOLUTE);
                }
                else
                {
                    LineDrawer.drawLine(topRight, bottomRight, Color.White, CoordinateTypeEnum.ABSOLUTE);
                }
                if (PositionsOwned.Contains(Direction.Down.Move(pt)))
                {
                    LineDrawer.drawLine(bottomLeft, bottomRight, Color.Blue, CoordinateTypeEnum.ABSOLUTE);
                }
                else
                {
                    LineDrawer.drawLine(bottomLeft, bottomRight, Color.White, CoordinateTypeEnum.ABSOLUTE);
                }

                // draw text on screen
                Vector2 boxCenter = topLeft + new Vector2(SCREEN_WIDTH_IN_PIXELS / 2 * scale, SCREEN_HEIGHT_IN_PIXELS / 2 * scale);
                Color color =
                    (this == GameplayManager.ActiveZone &&
                    this.getGlobalScreenFromPosition(GameplayManager.Samus.DrawPosition) == pt) ? Color.LimeGreen : Color.White;
                s_font.drawStringCentered(ScreenConstructionInfos[pt].Parameters.Label, boxCenter, color, 0.0f, Constants.DepthHUD);
            }

            foreach (IGameObject gameObject in GameObjects)
            {
                if (gameObject is Decoration)
                {
                    ((Decoration)gameObject).drawMap(offset, scale, depth);
                }

                if (gameObject is DoorController)
                {
                    ((DoorController)gameObject).drawMap(offset, scale, depth);
                }
            }
        }

        #endregion

        #region Static Helpers

        internal static void GetOctant(Vector2 positionInScreen, out Direction primary, out Direction secondary)
        {
            Direction topDown;
            Direction leftRight;
            if (positionInScreen.X <= Zone.SCREEN_WIDTH_IN_PIXELS / 2)
            {
                leftRight = Direction.Left;
            }
            else
            {
                leftRight = Direction.Right;

            }
            if (positionInScreen.Y <= Zone.SCREEN_HEIGHT_IN_PIXELS / 2)
            {
                topDown = Direction.Up;
            }
            else
            {
                topDown = Direction.Down;
            }

            if (leftRight == Direction.Left && topDown == Direction.Up && positionInScreen.X >= positionInScreen.Y ||
                leftRight == Direction.Left && topDown == Direction.Down && positionInScreen.X >= Zone.SCREEN_HEIGHT_IN_PIXELS - positionInScreen.Y ||
                leftRight == Direction.Right && topDown == Direction.Up && Zone.SCREEN_WIDTH_IN_PIXELS - positionInScreen.X >= positionInScreen.Y ||
                leftRight == Direction.Right && topDown == Direction.Down && Zone.SCREEN_WIDTH_IN_PIXELS - positionInScreen.X >= Zone.SCREEN_HEIGHT_IN_PIXELS - positionInScreen.Y)
            {
                primary = topDown;
                secondary = leftRight;
            }
            else
            {
                primary = leftRight;
                secondary = topDown;
            }
        }

        internal static Point SampleOctantPoint(Direction primary, Direction secondary)
        {
            Vector2 sample = SampleOctant(primary, secondary);
            return new Point((int)sample.X, (int)sample.Y);
        }

        internal static Vector2 SampleOctant(Direction primary, Direction secondary)
        {
            int leftMax = Math.Min(Zone.SCREEN_WIDTH_IN_PIXELS / 2, Zone.SCREEN_HEIGHT_IN_PIXELS / 2);
            int rightMin = Math.Max(Zone.SCREEN_WIDTH_IN_PIXELS / 2,
                                          Zone.SCREEN_WIDTH_IN_PIXELS - Zone.SCREEN_HEIGHT_IN_PIXELS / 2);
            int upMax = Math.Min(Zone.SCREEN_WIDTH_IN_PIXELS / 2, Zone.SCREEN_HEIGHT_IN_PIXELS / 2);
            int downMin = Math.Max(Zone.SCREEN_HEIGHT_IN_PIXELS / 2,
                                          Zone.SCREEN_HEIGHT_IN_PIXELS - Zone.SCREEN_WIDTH_IN_PIXELS / 2);

            double primaryCoord = 0;
            double secondaryCoord = 0;
            double primaryMin;
            double primaryMax;
            switch(primary.EnumValue)
            {
                case DirectionEnum.Left:
                    primaryMin = 0;
                    primaryMax = leftMax;
                    primaryCoord =
                        RandomManager.get().NextDouble() * (primaryMax - primaryMin) + primaryMin;
                    if (secondary == Direction.Up)
                    {
                        secondaryCoord = RandomManager.get().NextDouble() *
                            (leftMax - primaryCoord) +
                            primaryCoord;
                    }
                    else
                    {
                        secondaryCoord = RandomManager.get().NextDouble() *
                            (leftMax - primaryCoord) +
                            downMin;
                    }
                    break;
                case DirectionEnum.Up:
                    primaryMin = 0;
                    primaryMax = upMax;
                    primaryCoord =
                        RandomManager.get().NextDouble() * (primaryMax - primaryMin) + primaryMin;
                    if (secondary == Direction.Left)
                    {
                        secondaryCoord = RandomManager.get().NextDouble() *
                            (upMax - primaryCoord) +
                            primaryCoord;
                    }
                    else
                    {
                        secondaryCoord = RandomManager.get().NextDouble() *
                            (upMax - primaryCoord) +
                            rightMin;
                    }
                    break;
                case DirectionEnum.Right:
                    primaryMin = rightMin;
                    primaryMax = Zone.SCREEN_WIDTH_IN_PIXELS;
                    primaryCoord =
                        RandomManager.get().NextDouble() * (primaryMax - primaryMin) + primaryMin;
                    if (secondary == Direction.Up)
                    {
                        secondaryCoord = RandomManager.get().NextDouble() *
                            (upMax - (Zone.SCREEN_WIDTH_IN_PIXELS - primaryCoord)) +
                            (Zone.SCREEN_WIDTH_IN_PIXELS - primaryCoord);
                    }
                    else
                    {
                        secondaryCoord = RandomManager.get().NextDouble() *
                            (Zone.SCREEN_HEIGHT_IN_PIXELS - downMin - (Zone.SCREEN_WIDTH_IN_PIXELS - primaryCoord)) +
                            downMin;
                    }
                    break;
                case DirectionEnum.Down:
                    primaryMin = downMin;
                    primaryMax = Zone.SCREEN_HEIGHT_IN_PIXELS;
                    primaryCoord =
                        RandomManager.get().NextDouble() * (primaryMax - primaryMin) + primaryMin;
                    if (secondary == Direction.Left)
                    {
                        secondaryCoord = RandomManager.get().NextDouble() *
                            (leftMax - (Zone.SCREEN_HEIGHT_IN_PIXELS - primaryCoord)) +
                            (Zone.SCREEN_HEIGHT_IN_PIXELS - primaryCoord);
                    }
                    else
                    {
                        secondaryCoord = RandomManager.get().NextDouble() *
                            (Zone.SCREEN_WIDTH_IN_PIXELS - rightMin - (Zone.SCREEN_HEIGHT_IN_PIXELS - primaryCoord)) +
                            rightMin;
                    }
                    break;
            }

            if (primary.IsVertical)
            {
                double temp = primaryCoord;
                primaryCoord = secondaryCoord;
                secondaryCoord = temp;
            }
            return new Vector2((float)primaryCoord, (float)secondaryCoord);
        }

        #endregion
    }
}
