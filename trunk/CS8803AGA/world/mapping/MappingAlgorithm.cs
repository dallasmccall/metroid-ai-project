using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using QuestAdaptation.world.ga;

namespace QuestAdaptation.world.mapping
{
    /// <summary>
    /// A class that implements a tree to gridspace mapping algorithm
    /// </summary>
    abstract class MappingAlgorithm
    {
        /// <summary>
        /// Maps a RegionTree to a grid space.
        /// </summary>
        /// <param name="r">The RegionTree used for the mapping</param>
        /// <returns>Null if unsuccesful, a dictionary of Points & RegionTreeMarkers otherwise.</returns>
        public abstract Dictionary<Point, RegionMarker> mapToWorld(RegionTree r);
    }
}
