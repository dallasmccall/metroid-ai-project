using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using QuestAdaptation.world.ga;

namespace QuestAdaptation.world.mapping
{
    /// <summary>
    /// Maps a RegionTree object to a grid space.
    /// </summary>
    class RegionTreeMapper
    {
        #region fields
        private RegionTree tree { get; set; } //The RegionTree that is associated with this RegionTreeMapper
        private MappingAlgorithm ma { get; set; } //The MappingAlgorithm associated with this RegionTreeMapper
        #endregion

        #region Constants
        private const int RANDOM_SEED = 10; //The seed used for the random number generator
        #endregion

        #region Field Getters/Setters
        /// <summary>
        /// Getter and setter for the tree field
        /// </summary>
        public RegionTree Tree
        {
            get
            {
                return tree;
            }
            set
            {
                tree = value;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a RegionTreeMapper with the given RegionTree
        /// </summary>
        /// <param name="sol">The RegionTree that goes with this mapper</param>
        /// <param name="mapper">The MappingAlgorithm used to perform the mapping</param>
        public RegionTreeMapper(RegionTree sol, MappingAlgorithm mapAl)
        {
            tree = sol;
            ma = mapAl;
        }
        #endregion

        #region General Methods
        /// <summary>
        /// Maps a RegionTree to a grid space. Uses a Greedy algorithm
        /// </summary>
        /// <returns>Null if unsuccesful, a dictionary of Points & RegionTreeMarkers otherwise.</returns>
        public Dictionary<Point, RegionMarker> mapToWorld()
        {
            return ma.mapToWorld(tree);
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Gets the point to the North of the current location
        /// </summary>
        /// <param name="loc">The base location</param>
        /// <returns>The point North of the base location</returns>
        public static Point getNorth(Point loc)
        {
            return new Point(loc.X, loc.Y - 1);
        }

        /// <summary>
        /// Gets the point to the South of the current location
        /// </summary>
        /// <param name="loc">The base location</param>
        /// <returns>The point South of the base location</returns>
        public static Point getSouth(Point loc)
        {
            return new Point(loc.X, loc.Y + 1);
        }

        /// <summary>
        /// Gets the point to the East of the current location
        /// </summary>
        /// <param name="loc">The base location</param>
        /// <returns>The point East of the base location</returns>
        public static Point getEast(Point loc)
        {
            return new Point(loc.X + 1, loc.Y);
        }

        /// <summary>
        /// Gets the point to the West of the current location
        /// </summary>
        /// <param name="loc">The base location</param>
        /// <returns>The point West of the base location</returns>
        public static Point getWest(Point loc)
        {
            return new Point(loc.X - 1, loc.Y);
        }
        #endregion
        
    }
}
