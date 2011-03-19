using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidAI.controllers;
using MetroidAI.world.space;

namespace MetroidAI.world
{
    #region Public Helper Classes

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

    /// <summary>
    /// Intermediate class for instantiating zones
    /// Contains integers for the points at which borders enter/exit the zone; these are
    ///     used as endpoints for fractals so that the borders on adjacent zones/areas connect
    /// </summary>
    public class ScreenConstructionInfo
    {
        public Point Location { get; set; }
        public ScreenConstructionParameters Parameters { get; set; }

        internal Dictionary<Direction, int> OutsidePts = new Dictionary<Direction, int>();

        internal Dictionary<Direction, int> PathPts = new Dictionary<Direction, int>();

        internal Dictionary<Direction, Dictionary<Direction, int>> EdgePts =
            new Dictionary<Direction, Dictionary<Direction, int>>();

        internal Dictionary<Direction, Dictionary<Direction, int>> DoorPts =
            new Dictionary<Direction, Dictionary<Direction, int>>();

        internal Dictionary<Direction, Dictionary<Direction, Point>> InnerPts =
            new Dictionary<Direction, Dictionary<Direction, Point>>();

        public List<LineFractalInfo> Fractals { get; set; }

        public ScreenConstructionInfo(ScreenConstructionParameters parameters)
        {
            OutsidePts[Direction.Up] = -1;
            OutsidePts[Direction.Down] = Zone.SCREEN_HEIGHT_IN_PIXELS + 1;
            OutsidePts[Direction.Left] = -1;
            OutsidePts[Direction.Right] = Zone.SCREEN_WIDTH_IN_PIXELS + 1;

            EdgePts[Direction.Left] = new Dictionary<Direction, int>();
            EdgePts[Direction.Right] = new Dictionary<Direction, int>();
            EdgePts[Direction.Up] = new Dictionary<Direction, int>();
            EdgePts[Direction.Down] = new Dictionary<Direction, int>();

            DoorPts[Direction.Left] = new Dictionary<Direction, int>();
            DoorPts[Direction.Right] = new Dictionary<Direction, int>();
            DoorPts[Direction.Up] = new Dictionary<Direction, int>();
            DoorPts[Direction.Down] = new Dictionary<Direction, int>();

            InnerPts[Direction.Left] = new Dictionary<Direction, Point>();
            InnerPts[Direction.Right] = new Dictionary<Direction, Point>();
            InnerPts[Direction.Up] = new Dictionary<Direction, Point>();
            InnerPts[Direction.Down] = new Dictionary<Direction, Point>();

            this.Location = parameters.Location;
            this.Parameters = parameters;
            this.Fractals = new List<LineFractalInfo>();
        }
    }

    public enum Connection
    {
        None, Door, Open
    }

    #endregion

    public class ZoneBuilder
    {
        const int DOOR_SIZE = 160;

        #region Build the World!

        public static void BuildWorld(Dictionary<Point, ScreenConstructionParameters> markers)
        {
            Dictionary<Point, ScreenConstructionInfo> scis =
                InitializeScreenContruction2(markers);

            List<Zone> zones = createZones(scis);
        }

        #endregion

        #region Magical Functions of Craziness

        const int REGION_WIDTH = Zone.SCREEN_WIDTH_IN_PIXELS;
        const int REGION_HEIGHT = Zone.SCREEN_HEIGHT_IN_PIXELS;

        /// <summary>
        /// Converts ScreenMarker's into ScreenConstructionInfo's by creating endpoints for boundary lines
        /// </summary>
        /// <param name="screens"></param>
        /// <returns></returns>
        public static Dictionary<Point, ScreenConstructionInfo> InitializeScreenContruction
            (Dictionary<Point, ScreenConstructionParameters> screens)
        {
            Dictionary<Point, ScreenConstructionInfo> scis =
                new Dictionary<Point, ScreenConstructionInfo>();

            Random rand = RandomManager.get();

            foreach (ScreenConstructionParameters screen in screens.Values)
            {
                ScreenConstructionInfo sci = new ScreenConstructionInfo(screen);
                scis.Add(sci.Location, sci);

                foreach (Direction d in Direction.All)
                {
                    int maxDimension = d.IsVertical ? REGION_WIDTH : REGION_HEIGHT; // perpendicular to dir
                    int tileSize = d.IsVertical ? Zone.TILE_WIDTH : Zone.TILE_HEIGHT;
                    Direction positiveRotation = (d == Direction.Up || d == Direction.Right) ? d.RotationCW : d.RotationCCW;

                    Connection connection =
                        screen.Connections.ContainsKey(d) ? screen.Connections[d] : Connection.None;
                    Point adjacentPoint = DirectionUtils.Move(screen.Location, d);

                    if (connection != Connection.None && scis.ContainsKey(adjacentPoint))
                    {
                        ScreenConstructionInfo adjacent = scis[adjacentPoint];

                        sci.EdgePts[d][d.RotationCCW] = adjacent.EdgePts[d.Opposite][d.RotationCCW];
                        sci.EdgePts[d][d.RotationCW] = adjacent.EdgePts[d.Opposite][d.RotationCW];

                        sci.DoorPts[d][d.RotationCCW] = adjacent.DoorPts[d.Opposite][d.RotationCCW];
                        sci.DoorPts[d][d.RotationCW] = adjacent.DoorPts[d.Opposite][d.RotationCW];

                        sci.PathPts[d] = adjacent.PathPts[d.Opposite];
                    }
                    else
                    {
                        sci.EdgePts[d][positiveRotation.Opposite] = rand.Next(0, maxDimension / 4);
                        sci.EdgePts[d][positiveRotation] = rand.Next(maxDimension * 3 / 4, maxDimension);

                        sci.DoorPts[d][positiveRotation.Opposite] =
                            (maxDimension / 2 - DOOR_SIZE / 2) - (maxDimension / 2 - DOOR_SIZE / 2) % tileSize - tileSize / 2;
                        sci.DoorPts[d][positiveRotation] =
                            (maxDimension / 2 + DOOR_SIZE / 2) - (maxDimension / 2 + DOOR_SIZE / 2) % tileSize + tileSize / 2;

                        sci.PathPts[d] = maxDimension / 2;
                    }
                }
            }

            return scis;
        }

        public static Dictionary<Point, ScreenConstructionInfo> InitializeScreenContruction2
            (Dictionary<Point, ScreenConstructionParameters> screens)
        {
            Dictionary<Point, ScreenConstructionInfo> scis =
                new Dictionary<Point, ScreenConstructionInfo>();

            Random rand = RandomManager.get();

            foreach (ScreenConstructionParameters screen in screens.Values)
            {
                ScreenConstructionInfo sci = new ScreenConstructionInfo(screen);
                scis.Add(sci.Location, sci);

                foreach (Direction d in Direction.All)
                {
                    int maxDimension = d.IsVertical ? REGION_WIDTH : REGION_HEIGHT; // perpendicular to dir
                    int tileSize = d.IsVertical ? Zone.TILE_WIDTH : Zone.TILE_HEIGHT;
                    Direction positiveRotation =
                        (d == Direction.Up || d == Direction.Right) ? d.RotationCW : d.RotationCCW;

                    Connection connection =
                        screen.Connections.ContainsKey(d) ? screen.Connections[d] : Connection.None;
                    Point adjacentPoint = DirectionUtils.Move(screen.Location, d);

                    if (connection != Connection.None && scis.ContainsKey(adjacentPoint))
                    {
                        ScreenConstructionInfo adjacent = scis[adjacentPoint];

                        sci.EdgePts[d][d.RotationCCW] = adjacent.EdgePts[d.Opposite][d.RotationCCW];
                        sci.EdgePts[d][d.RotationCW] = adjacent.EdgePts[d.Opposite][d.RotationCW];

                        sci.DoorPts[d][d.RotationCCW] = adjacent.DoorPts[d.Opposite][d.RotationCCW];
                        sci.DoorPts[d][d.RotationCW] = adjacent.DoorPts[d.Opposite][d.RotationCW];

                        sci.PathPts[d] = adjacent.PathPts[d.Opposite];
                    }
                    else
                    {
                        sci.EdgePts[d][positiveRotation.Opposite] = maxDimension / 8;
                        sci.EdgePts[d][positiveRotation] = maxDimension * 7 / 8;

                        sci.DoorPts[d][positiveRotation.Opposite] =
                            (maxDimension / 2 - DOOR_SIZE / 2) - (maxDimension / 2 - DOOR_SIZE / 2) % tileSize - tileSize / 2;
                        sci.DoorPts[d][positiveRotation] =
                            (maxDimension / 2 + DOOR_SIZE / 2) - (maxDimension / 2 + DOOR_SIZE / 2) % tileSize + tileSize / 2;

                        sci.PathPts[d] = maxDimension / 2;
                    }
                }
            }

            return scis;
        }

        public static void swap(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        static void swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        #endregion

        #region Zone Creation

        /// <summary>
        /// Instantiates zones from ScreenConstructionInfo's by checking their shapes and adding appropriate
        /// boundary fractals
        /// </summary>
        /// <param name="rcis"></param>
        /// <returns></returns>
        public static List<Zone> createZones(Dictionary<Point, ScreenConstructionInfo> scis)
        {
            List<List<ScreenConstructionInfo>> zoneLists =
                new List<List<ScreenConstructionInfo>>();

            HashSet<Point> visitedScreens = new HashSet<Point>();

            foreach (ScreenConstructionInfo sci in scis.Values)
            {
                if (visitedScreens.Contains(sci.Location)) continue;

                List<ScreenConstructionInfo> curZone = new List<ScreenConstructionInfo>();

                createZoneDFS(scis, sci, curZone, visitedScreens);
                zoneLists.Add(curZone);
            }

            List<Zone> zones = new List<Zone>();

            foreach (List<ScreenConstructionInfo> zoneList in zoneLists)
            {
                zones.Add(constructZone(zoneList));
            }

            return zones;
        }

        private static void createZoneDFS(
            Dictionary<Point, ScreenConstructionInfo> scis,
            ScreenConstructionInfo curScreen,
            List<ScreenConstructionInfo> curZone,
            HashSet<Point> visitedScreens)
        {
            if (visitedScreens.Contains(curScreen.Location))
            {
                if (!curZone.Contains(curScreen))
                {
                    throw new Exception("I think a screen is being added to more than 1 zone?");
                }

                return;
            }

            curZone.Add(curScreen);
            visitedScreens.Add(curScreen.Location);

            foreach (Direction d in Direction.All)
            {
                Connection c;
                if (curScreen.Parameters.Connections.TryGetValue(d, out c) && c == Connection.Open)
                {
                    ScreenConstructionInfo adjScreen = scis[DirectionUtils.Move(curScreen.Location, d)];
                    createZoneDFS(scis, adjScreen, curZone, visitedScreens);
                }
            }
        }

        public static Zone constructZone(List<ScreenConstructionInfo> screens)
        {
            // Select an environment type and make zone
            Zone zone;
            Random r = RandomManager.get();
            if (r.NextDouble() > 0.75)
                zone = Zone.makeEmptyZone("Sprites/Crateria", screens);
            else if (r.NextDouble() > 0.67)
                zone = Zone.makeEmptyZone("Sprites/WreckedShip", screens);
            else if (r.NextDouble() > 0.5)
                zone = Zone.makeEmptyZone("Sprites/LowerBrinstar", screens);
            else
                zone = Zone.makeEmptyZone("Sprites/Chozo", screens);

            foreach (ScreenConstructionInfo sci in screens)
            {
                // Add doors on each side they're needed
                foreach (Direction d in Direction.All)
                {
                    Connection c;
                    if (sci.Parameters.Connections.TryGetValue(d, out c) && c == Connection.Door)
                    {
                        zone.add(new DoorController(d, zone, zone.getLocalScreenFromGlobalScreen(sci.Location)));
                    }
                }
            }

            return zone;
        }

        #endregion

        #region Zone Population

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
            efi.FILL_BOTTOMLEFT = 2; // 18
            efi.FILL_BOTTOMRIGHT = 0; // 16
            efi.FILL_LEFT = 7;
            efi.FILL_RIGHT = 5;
            efi.FILL_TOP = 11;
            efi.FILL_TOPLEFT = 12; // 15
            efi.FILL_TOPRIGHT = 10; //17
            efi.OTHER = 4;

            return efi;
        }

        #endregion
    }
}
