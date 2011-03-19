using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.world.space;
using MetroidAI.world.mission;

namespace MetroidAI.world.space
{
    class MissionQueueSimple : IMissionQueue
    {
        protected int m_terminalsRemaining = 0;
        protected Dictionary<IMissionTerminalExpander, MissionNode> m_readyTerminals =
            new Dictionary<IMissionTerminalExpander, MissionNode>();

        protected Dictionary<MissionNode, int> m_inEdgeCounts =
            new Dictionary<MissionNode, int>();
        protected Dictionary<MissionNode, List<MissionNode>> m_outEdges =
            new Dictionary<MissionNode, List<MissionNode>>();

        public MissionQueueSimple(MissionQueueSimple other)
        {
            this.m_terminalsRemaining = other.m_terminalsRemaining;
            this.m_readyTerminals = new Dictionary<IMissionTerminalExpander, MissionNode>(other.m_readyTerminals);
            this.m_inEdgeCounts = new Dictionary<MissionNode, int>(other.m_inEdgeCounts);
            this.m_outEdges = new Dictionary<MissionNode, List<MissionNode>>(other.m_outEdges);
        }

        public MissionQueueSimple(MissionImpl mission)
        {
            Dictionary<int, MissionNode> IDs = new Dictionary<int, MissionNode>();
            foreach (MissionNode node in mission.MissionNodes)
            {
                if (!IDs.ContainsKey(node.ID))
                {
                    IDs.Add(node.ID, node);

                    m_terminalsRemaining++;
                    m_inEdgeCounts.Add(node, 0);
                    m_outEdges.Add(node, new List<MissionNode>());
                }
            }

            foreach (MissionNode node in IDs.Values)
            {
                foreach (MissionConnection connection in node.Connections)
                {
                    if (connection.SourceID == node.ID ||
                        connection.DestID == node.ID)
                    {
                        MissionNode src = IDs[connection.SourceID];
                        MissionNode dest = IDs[connection.DestID];
                        m_inEdgeCounts[dest]++;
                        m_outEdges[src].Add(dest);

                        if (connection.Edge == MissionEdge.Linear)
                        {
                            dest.Expander.AddLinearRequirement(src.ID.ToString());
                        }
                    }
                    else
                    {
                        throw new Exception("Unknown error in MissionQueueSimple...");
                    }
                }
            }

            foreach (MissionNode node in IDs.Values)
            {
                if (m_inEdgeCounts[node] == 0)
                {
                    m_readyTerminals.Add(node.Expander, node);
                }
            }
        }

        #region IMissionQueue Members

        public int Count
        {
            get { return m_terminalsRemaining; }
        }

        public List<IMissionTerminalExpander> ReadyTerminals
        {
            get { return new List<IMissionTerminalExpander>(m_readyTerminals.Keys); }
        }

        public IMissionQueue DeepCopy()
        {
            return new MissionQueueSimple(this);
        }

        public void MarkTerminal(IMissionTerminalExpander terminal)
        {
            foreach (MissionNode dest in m_outEdges[m_readyTerminals[terminal]])
            {
                m_inEdgeCounts[dest] -= 1;
                if (m_inEdgeCounts[dest] == 0)
                {
                    m_readyTerminals.Add(dest.Expander, dest);
                }
            }

            m_terminalsRemaining--;
            m_readyTerminals.Remove(terminal);
        }

        #endregion
    }
}
