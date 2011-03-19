using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CS8803AGA.engine;
using CS8803AGA.world;

namespace CS8803AGA
{
    /// <summary>
    /// Static class for storing the Areas and Regions of the world
    /// TODO - make a singleton or store in a GameplayState
    /// </summary>
    public static class WorldManager
    {
        private static Dictionary<Point, Zone> ZoneMap;
        private static HashSet<Zone> Zones;

        static WorldManager()
        {
            ZoneMap = new Dictionary<Point, Zone>();
            Zones = new HashSet<Zone>();
        }

        public static Zone GetZone(Point p)
        {
            if (ZoneMap.ContainsKey(p))
                return ZoneMap[p];

            return null;
        }

        public static IEnumerable<Zone> GetZones()
        {
            return ZoneMap.Values;
        }

        public static void RegisterZone(Zone zone)
        {
            foreach (Point pt in zone.PositionsOwned)
            {
                if (ZoneMap.ContainsKey(pt))
                    throw new Exception(String.Format("Zone at {0},{1} already registered", pt.X, pt.Y));

                ZoneMap[pt] = zone;
                Zones.Add(zone);
            }
        }

        /// <summary>
        /// Draws a map to the screen.
        /// </summary>
        /// <param name="corner">Top left corner the map should be placed, in pixels.</param>
        /// <param name="mapWidth">Size available for map width, in pixels.</param>
        /// <param name="mapHeight">Size available for map height, in pixels.</param>
        /// <param name="depth">Drawing depth.</param>
        public static void DrawMap(Vector2 corner, float mapWidth, float mapHeight, float depth)
        {
            //int minX = int.MaxValue;
            //int minY = int.MaxValue;
            //int maxX = int.MinValue;
            //int maxY = int.MinValue;

            //foreach (Point p in WorldMap.Keys)
            //{
            //    minX = Math.Min(p.X, minX);
            //    minY = Math.Min(p.Y, minY);
            //    maxX = Math.Max(p.X, maxX);
            //    maxY = Math.Max(p.Y, maxY);
            //}

            //float scale = Math.Min(mapWidth / (Area.WIDTH_IN_TILES * Area.TILE_WIDTH * (maxX - minX + 1)),
            //                        mapHeight / (Area.HEIGHT_IN_TILES * Area.HEIGHT_IN_TILES * (maxY - maxY + 1)));

            float scale = 1.0f / 4;
            float screenSizeX = Zone.SCREEN_WIDTH_IN_PIXELS * scale;
            float screenSizeY = Zone.SCREEN_HEIGHT_IN_PIXELS * scale;

            Zone activeZone = GameplayManager.ActiveZone;
            Point globalScreenCoord = activeZone.getGlobalScreenFromPosition(GameplayManager.Samus.DrawPosition);
            Vector2 activeZoneOffset = new Vector2(screenSizeX * globalScreenCoord.X + screenSizeX/2,
                                    screenSizeY * globalScreenCoord.Y + screenSizeY/2);

            foreach (Zone z in Zones)
            {
                Vector2 zoneTopLeftCorner =
                        new Vector2(screenSizeX * z.TopLeftPosition.X + corner.X - activeZoneOffset.X,
                                    screenSizeY * z.TopLeftPosition.Y + corner.Y - activeZoneOffset.Y);

                z.drawMap(zoneTopLeftCorner, scale, Constants.DepthGameplayTiles);
            }
        }

        public static void reset()
        {
            ZoneMap.Clear();
            Zones.Clear();
        }
    }
}
