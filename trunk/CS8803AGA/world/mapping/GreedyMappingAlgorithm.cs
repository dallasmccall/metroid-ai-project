using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using QuestAdaptation.world.ga;

namespace QuestAdaptation.world.mapping
{
    /// <summary>
    /// A greedy mapping algorithm. Places each node one step at a time, without regard for future conflicts.
    /// </summary>
    class GreedyMappingAlgorithm : MappingAlgorithm
    {
        /// <summary>
        /// Maps a RegionTree to a grid space. Uses a Greedy algorithm.
        /// </summary>
        /// <param name="r">The RegionTree used for the mapping</param>
        /// <returns>Null if unsuccesful, a dictionary of Points & RegionTreeMarkers otherwise.</returns>
        public override Dictionary<Point, RegionMarker> mapToWorld(RegionTree tree)
        {
            Dictionary<Point, RegionMarker> markers = new Dictionary<Point, RegionMarker>(); //will contain the RegionMarker objects for this world
            Dictionary<RegionTreeNode, Point> parents = new Dictionary<RegionTreeNode, Point>(); //will contain every node that has already been given a location

            List<RegionTreeNode> queue = new List<RegionTreeNode>(); //The list of nodes to be processed
            Random generator = new Random(10); //Initializes a random number generator

            queue.Add(tree.root); //Adds the root node to the queue

            //While there are still nodes in the queue
            while (queue.Count > 0)
            {
                RegionTreeNode currNode = queue.ElementAt(0); //The current node is the head of the queue
                RegionTreeNode parent = currNode.Parent; //The parent of the current node
                queue.AddRange(currNode.children); //Adds the children of the current node to the queue
                queue.Remove(currNode); //Removes the current node from the queue

                if (parent == null) //If the current node is the root of the tree
                {
                    //Place root node at the origin
                    Point loc = new Point(0, 0);
                    RegionMarker marker = new RegionMarker(loc, false, false, false, false, currNode.region.environment.getName());
                    parents.Add(currNode, loc);
                    markers.Add(loc, marker);
                }
                else
                {
                    //If the parent node of the current node is not in the Dictionary already, throw an Exception. Should never occur
                    if (!parents.ContainsKey(parent))
                        throw new Exception("Parent not in Dictionary");

                    Point parLoc = parents[parent]; //Gets the location of this node's parent node
                    bool success = false; //whether or not a placement can be found for this node
                    List<int> tried = new List<int>(); //The directions that have been tried so far

                    //Attempts to place the child node in one of the four directions, randomly chosen until succesful
                    while (!success || tried.Count < 4)
                    {
                        int direction = generator.Next(4); //Randomly generated direction
                        if (!tried.Contains(direction)) //If this direction has not already been done, add it to the list of tried directions
                            tried.Add(direction);

                        Point placement = parLoc; //The location of the placement. Initialized to the parent location
                        if (direction == 0) placement = RegionTreeMapper.getNorth(parLoc); //If north
                        else if (direction == 1) placement = RegionTreeMapper.getSouth(parLoc); //If south
                        else if (direction == 2) placement = RegionTreeMapper.getEast(parLoc); //If east
                        else if (direction == 3) placement = RegionTreeMapper.getWest(parLoc); //If west

                        success = !markers.ContainsKey(placement); //If the location of the chosen direction already has a node in it

                        if (success) //If the location is available
                        {
                            RegionMarker parentMarker = markers[parLoc]; //Get the marker of the parent
                            RegionMarker childMarker = new RegionMarker(placement, false, false, false, false, currNode.region.environment.getName()); //initializes a RegionMarker for this node
                            if (direction == 0) //If North
                            {
                                parentMarker.connectsTop = true; //Parent is connected at the top
                                childMarker.connectsBottom = true; //Child is connected at the bottom
                            }
                            else if (direction == 1) //If South
                            {
                                parentMarker.connectsBottom = true; //Parent is connected at the bottom
                                childMarker.connectsTop = true; //Child is connected at the top
                            }
                            else if (direction == 2) //If East
                            {
                                parentMarker.connectsRight = true; //Parent is connected at the right
                                childMarker.connectsLeft = true; //Child is connected at the left
                            }
                            else if (direction == 3) //If West
                            {
                                parentMarker.connectsLeft = true; //Parent is connected at the left
                                childMarker.connectsRight = true; //Child is connected at the right
                            }

                            parents.Add(currNode, placement); //Add the newly created node's location to the parents Dictionary
                            markers.Add(placement, childMarker); //Adds the newly created node to the markers
                        }
                        else //If the location is unavailable, the algorithm failed and returns null
                            return null;
                    }

                }
            }
            return markers;
        }
    }
}
