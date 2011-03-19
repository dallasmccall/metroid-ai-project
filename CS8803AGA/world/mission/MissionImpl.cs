using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.world.space;
using MetroidAI.controllers;

namespace MetroidAI.world.mission
{
    /// <summary>
    /// Contains a graph of a mission, possibly with some non-terminal nodes.
    /// Thus it can be used by MissionGrammarImpl as an in-progress walk of
    /// its encapsulated grammar.
    /// </summary>
    class MissionImpl : IMission
    {
        // Could have a root MissionNode with outedges, thus walking the tree like, well, a tree
        // Or could use a list of all the connections, or a dictionary of MissionNodes to
        //  lists of connections... I guess those theory people would call this an "adjacency list"
        // Or simply custom data structures to better manage where non-terminal nodes are.

        // Needs to expose non-terminal nodes so that the grammar can check its rules against them.

        // Needs to expose a way for the grammar to actually fire its rules and change underlying graph.

        #region IMission Members

        public MetroidAI.world.space.IMissionQueue MissionQueue
        {
            get
            {
                // Might actually be easiest to just make this implement
                // IMissionQueue and have it return itself.
                //throw new NotImplementedException();
                return new MissionQueueSimple(this);
            }
        }

        #endregion

        public List<MissionNode> MissionNodes { get; set; }
        public List<ParamContainer> ParamContainers { get; set; }

        public MissionImpl()
        {
            MissionNodes = new List<MissionNode>();
            ParamContainers = new List<ParamContainer>();
        }

        public void PostProcess()
        {
            foreach (ParamContainer pc in ParamContainers)
            {
                switch (pc.Key)
                {
                    default:
                    case "Item":
                        pc.Value = (RandomManager.get().NextDouble() < 0.5) ? Item.MorphingBall.ToString() : Item.IceBeam.ToString();
                        break;
                }
            }

            foreach (MissionNode node in MissionNodes)
            {
                foreach (ParamContainer pc in node.ParamContainers)
                {
                    node.Expander.AddParameter(pc.Key, pc.Value);
                }

                node.Expander.InitializeParams();
            }
        }
    }

    class MissionNode
    {
        private static List<int> freeIDs = new List<int>();
        private static int currentID = 0;

        public static int getNewID()
        {
            int ID = currentID;
            if (freeIDs.Count > 0)
            {
                ID = freeIDs[0];
                freeIDs.RemoveAt(0);
                return ID;
            }
            currentID += 1;
            return ID;
        }

        public void freeID()
        {
            freeIDs.Add(this.ID);
        }

        public void printNode()
        {
            Console.WriteLine("Name: " + Name + " ID: " + ID);
            Console.WriteLine("Connections:");
            foreach (MissionConnection conn in Connections)
            {
                Console.WriteLine("\t(" +conn.SourceID + "," + conn.DestID + ") Type: " + conn.Edge);
            }
        }

        public MissionNode(string name) : this(name, new List<ParamContainer>())
        {
            // nch
        }

        public MissionNode(string name, List<ParamContainer> parentParams)
        {
            this.Name = name;

            this.ID = getNewID();
            this.Connections = new List<MissionConnection>();
            this.ParamContainers = new List<ParamContainer>(parentParams);
        }

        public string expansionID { get; set; } //ID to be used only in expanding the grammar
        public int ID { get; set; } // Don't know if this will be useful
        public string Name { get; set; } // String used as an identifier for grammar rules
        public IMissionTerminalExpander Expander { get; set; } // Used to convert terminal into space
        public List<ParamContainer> ParamContainers { get; set; } // Parameters for a given expansion which
                                                                  // propagate down tree into the terminals

        public List<MissionConnection> Connections { get; set; }
        // Could have a list of connections as out edges.

    }

    public enum MissionEdge
    {
        Parallel,             // A -> B means A comes before B in the mission, but possibly parallel in space
        Linear                // A -> B means A comes before B in both the mission and the space
    }

    public class ParamContainer
    {
        public String Key { get; set; }
        public String Value { get; set; }

        public ParamContainer(String key)
        {
            this.Key = key;
        }
    }

    public class MissionConnection
    {
        public int SourceID { get; set; }
        public int DestID { get; set; }
        public MissionEdge Edge { get; set; }

        public MissionConnection(int source, int dest, MissionEdge edge)
        {
            this.SourceID = source;
            this.DestID = dest;
            this.Edge = edge;
        }
    }
}
