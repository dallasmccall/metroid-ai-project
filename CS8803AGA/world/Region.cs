using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA
{
    /// <summary>
    /// A 2D array of Areas in the world which share an environment type.
    /// </summary>
    public class Region
    {
        public Point location;          // global coordinate in world, on a region-scale
        //public Environment environment; // type or environment for the region

        public const int AREAS_WIDE = 1;
        public const int AREAS_TALL = 1;

        // when creating regions, what initially happens is a space of these sizes is
        //  created and fractals are put into it; this space is then scaled to the appropriate
        //  number of areas in the region, and scaled to the size of the areas
        public const int CONSTRUCTION_WIDTH = 1000;
        public const int CONSTRUCTION_HEIGHT = 1000;

        public Area[,] areas; // areas within the region, accessible by x,y coordinates

        public Region(Point location/*, Environment environment*/)
        {
            this.location = location;
            //this.environment = environment;

            this.areas = new Area[AREAS_WIDE, AREAS_TALL];
        }

        /// <summary>
        /// Returns the location of a Region that a pt is actually located in.
        /// </summary>
        /// <param name="pt">The point whose true Region we want.</param>
        /// <param name="currentRegion">Region whose coordinate system the point is in.</param>
        /// <returns>Location of the actual Region the point is located in.</returns>
        public static Point getPointsRegion(Vector2 pt, Point currentRegion)
        {
            int x = (int)Math.Floor(pt.X / Region.CONSTRUCTION_WIDTH) + currentRegion.X;
            int y = (int)Math.Floor(pt.Y / Region.CONSTRUCTION_HEIGHT) + currentRegion.Y;

            return new Point(x, y);
        }

        /// <summary>
        /// Lossily moves a point into the bounds of a Region; use getPointsActualRegion first.
        /// </summary>
        /// <param name="pt">The point to be moved.</param>
        /// <returns>Translation of the point into valid Region coordinates.</returns>
        public static Vector2 mapCoordToRegion(Vector2 pt)
        {
            // TODO find a better way to take care of negative modulo
            while (pt.X < 0)
            {
                pt.X += Region.CONSTRUCTION_WIDTH;
            }
            while (pt.Y < 0)
            {
                pt.Y += Region.CONSTRUCTION_HEIGHT;
            }
            return new Vector2(pt.X % Region.CONSTRUCTION_WIDTH, pt.Y % Region.CONSTRUCTION_HEIGHT);
        }

        /// <summary>
        /// Determines the global Area location of a point in a Region's coordinate system.
        /// </summary>
        /// <param name="pt">Point whose global Area location we want.</param>
        /// <param name="currentRegion">Region whose coordinates the point is in.</param>
        /// <returns>The global location of the Area the point lies in.</returns>
        public static Point getPointsArea(Vector2 pt, Point currentRegion)
        {
            Point trueRegion = getPointsRegion(pt, currentRegion);
            Vector2 trueRegionCoord = mapCoordToRegion(pt);

            int areaOffsetX = (int)trueRegionCoord.X / (Region.CONSTRUCTION_WIDTH / Region.AREAS_WIDE) + trueRegion.X * Region.AREAS_WIDE;
            int areaOffsetY = (int)trueRegionCoord.Y / (Region.CONSTRUCTION_HEIGHT / Region.AREAS_TALL) + trueRegion.Y * Region.AREAS_TALL;

            return new Point(areaOffsetX, areaOffsetY);
        }

        /// <summary>
        /// Converts a pt from the Region's coordinate system to an Area's coordinate system.
        /// </summary>
        /// <param name="pt">Point to be mapped from Region to Area.</param>
        /// <param name="areaX">Area's X location WITHIN THE REGION.</param>
        /// <param name="areaY">Area's Y location WITHIN THE REGION.</param>
        /// <returns>Point's location relative to the Area's coordinate system.</returns>
        public static Vector2 mapCoordToArea(Vector2 pt, int areaX, int areaY)
        {
            const float scaleX = (float)(Area.WIDTH_IN_TILES * Area.TILE_WIDTH) * Region.AREAS_WIDE / Region.CONSTRUCTION_WIDTH;
            const float scaleY = (float)(Area.HEIGHT_IN_TILES * Area.TILE_HEIGHT) * Region.AREAS_TALL / Region.CONSTRUCTION_HEIGHT;

            float transX = -areaX * (Area.WIDTH_IN_TILES * Area.TILE_WIDTH);
            float transY = -areaY * (Area.HEIGHT_IN_TILES * Area.TILE_HEIGHT);

            return new Vector2(pt.X * scaleX + transX, pt.Y * scaleY + transY);
        }

        /// <summary>
        /// Lossily converts a global Area location into its location relative to its Region.
        /// </summary>
        /// <param name="areaLoc">Area's global location.</param>
        /// <returns>Location of the Area relative to the Region in which it is located.</returns>
        public static Point getAreasLocInRegion(Point areaLoc)
        {
            // TODO find a better way to handle negative modulo
            while (areaLoc.X < 0)
                areaLoc.X += Region.AREAS_WIDE;
            while (areaLoc.Y < 0)
                areaLoc.Y += Region.AREAS_TALL;
            return new Point(areaLoc.X % Region.AREAS_WIDE, areaLoc.Y % Region.AREAS_TALL);
        }
    }
}
