using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TestHarness
{
    /// <summary>
    /// Deprecated - builds a world with an algorithm, not using randomized optimization
    /// </summary>
    public class WorldBuilder
    {
        public static void test()
        {
            List<string> islands = new List<string>();
            islands.Add("castle");
            islands.Add("forest");
            islands.Add("cave");

            WorldBuilder wb = new WorldBuilder();
            wb.buildIslandsAndBridges(islands);
        }

        private Random rand;
        private static Dictionary<string, AreaNode> IslandMap;
        private static Dictionary<Point, AreaNode> WorldMap;

        public WorldBuilder()
        {
            rand = new Random();
            IslandMap = new Dictionary<string, AreaNode>();
            WorldMap = new Dictionary<Point, AreaNode>();
        }

        public void buildIslandsAndBridges(List<string> islandNames)
        {
            string prev = "";
            string cur = "";
            while (islandNames.Count != 0)
            {
                prev = cur;
                cur = islandNames[0];
                islandNames.RemoveAt(0);

                // first island
                if (prev == "")
                {
                    AreaNode n = new AreaNode();
                    n.name = cur;
                    n.loc = new Point(0, 0);
                    n.type = AREA_TYPE.ISLAND;

                    IslandMap.Add(n.name, n);
                    WorldMap.Add(n.loc, n);
                }
                else
                {
                    List<Point> bridge = search(prev, cur);
                    foreach (Point p in bridge)
                    {
                        AreaNode n = new AreaNode();
                        n.name = prev + ":" + cur;
                        n.loc = p;
                        n.type = AREA_TYPE.BRIDGE;

                        //IslandMap.Add(n.name, n);
                        WorldMap.Add(n.loc, n);
                    }
                }
            }
        }

        public List<Point> search(string startIsland, string destIsland)
        {
            if (IslandMap.ContainsKey(destIsland))
            {
                return search(IslandMap[startIsland], IslandMap[destIsland]);
            }

            List<Point> bridgePlusDest = search(IslandMap[startIsland], destIsland);
            Point destPt = bridgePlusDest[bridgePlusDest.Count - 1];
            bridgePlusDest.RemoveAt(bridgePlusDest.Count - 1);

            AreaNode n = new AreaNode();
            n.name = destIsland;
            n.loc = destPt;
            n.type = AREA_TYPE.ISLAND;

            IslandMap.Add(n.name, n);
            WorldMap.Add(n.loc, n);

            return bridgePlusDest;
        }

        // pseudo-random search to build a bridge between two islands already on map
        private List<Point> search(AreaNode startIsland, AreaNode endIsland)
        {
            throw new Exception("not impled");
        }

        // psuedo-random search to build a bridge to a new island to be placed on map
        private List<Point> search(AreaNode startIsland, string destIsland)
        {
            //AreaNode destNode = new AreaNode();
            //destNode.name = destIsland;

            SortedList<float, SearchNode> openList = new SortedList<float, SearchNode>();

            SearchNode searchNode = new SearchNode();
            searchNode.loc = startIsland.loc;
            searchNode.wt = 0;
            searchNode.prev = new SearchNode(); // prevents null ptr errors
            searchNode.prev.loc = searchNode.loc; // shouldn't affect result

            const int THRESHOLD = 6;
            while (evaluateIslandPos(searchNode.loc) + rand.Next(3) < THRESHOLD)
            {
                foreach (SearchNode x in expand(searchNode))
                {
                    openList.Add(x.wt + (float)rand.NextDouble(), x);
                }

                if (openList.Count == 0)
                    throw new Exception("Map construction failure... not good");

                searchNode = openList[openList.Keys[0]];
                openList.Remove(openList.Keys[0]);
            }

            List<Point> finalPath = new List<Point>();
            while (searchNode.loc != startIsland.loc)
            {
                finalPath.Insert(0, searchNode.loc);
                searchNode = searchNode.prev;
            }
            return finalPath;
        }

        private int evaluateIslandPos(Point p)
        {
            if (WorldMap.ContainsKey(p))
            {
                return Int32.MinValue;
            }

            int mindistance = Int32.MaxValue;
            foreach (AreaNode island in IslandMap.Values)
            {
                int dist = Math.Abs(island.loc.X - p.X) +
                            Math.Abs(island.loc.Y - p.Y);
                if (dist < mindistance)
                    mindistance = dist;
            }

            return mindistance;
        }

        private List<SearchNode> expand(SearchNode cur)
        {
            Point west = new Point(cur.loc.X - 1, cur.loc.Y);
            Point east = new Point(cur.loc.X + 1, cur.loc.Y);
            Point north = new Point(cur.loc.X, cur.loc.Y - 1);
            Point south = new Point(cur.loc.X, cur.loc.Y + 1);

            List<Point> test = new List<Point>();
            test.Add(west);
            test.Add(east);
            test.Add(north);
            test.Add(south);

            List<SearchNode> expansion = new List<SearchNode>();
            foreach (Point p in test)
            {
                if (!WorldMap.ContainsKey(p) && p != cur.prev.loc)
                {
                    int cost;
                    // if moving in same direction, cheaper
                    int dx = cur.loc.X - cur.prev.loc.Y;
                    int dy = cur.loc.Y - cur.prev.loc.Y;
                    if (dx == (p.X - cur.loc.X) && dy == (p.Y - cur.loc.Y))
                    {
                        cost = 10 + rand.Next(10);
                    }
                    else
                    {
                        cost = 15 + rand.Next(10);
                    }

                    SearchNode s = new SearchNode();
                    s.wt = cur.wt + cost;
                    s.prev = cur;
                    s.loc = p;
                    expansion.Add(s);
                }
            }
            return expansion;
        }

        private class SearchNode
        {
            public Point loc;
            public SearchNode prev;
            public int wt;
        }

    }

    public enum AREA_TYPE
    {
        ISLAND, BRIDGE
    }

    public class AreaNode
    {
        public AREA_TYPE type;
        public Point loc;

        public List<int> ids;

        public string name;

        public AreaNode()
        {
            ids = new List<int>();
        }
    }
}
