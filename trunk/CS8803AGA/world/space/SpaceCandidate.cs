using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI.world.space
{
    class SpaceCandidate : ISpaceCandidate
    {
        float m_cost = 0.0f;
        ISpace m_space;
        IMissionQueue m_mission;

        protected SpaceCandidate()
        {
            // nch, only used for copy
        }

        public SpaceCandidate(IMissionQueue mission)
        {
            m_space = new SpaceImpl(new Point(0,0));
            m_mission = mission;
        }

        #region ISpaceCandidate Members

        public ISpace Space
        {
            get { return m_space; }
        }

        public IMissionQueue MissionQueue
        {
            get { return m_mission; }
        }

        public float Cost
        {
            get { return m_cost; }
        }

        public List<ISpaceCandidate> Expand()
        {
            List<ISpaceCandidate> successors = new List<ISpaceCandidate>();

            int childCount = 0;

            foreach (IMissionTerminalExpander terminal in m_mission.ReadyTerminals.Where(i => i.LinearReqs.Count > 0))
            {
                foreach (KeyValuePair<CoordPair, Connection> openSlot in (m_space as SpaceImpl).OpenConnectionsRandom)
                {
                    Point sourcePos = openSlot.Key.SourceCoord;
                    Point destPos = openSlot.Key.DestCoord;

                    // TODO
                    // possible optimization: keep hash set of openSlot/terminal pairs
                    //  which have been tried and failed so we don't try them again in successors;
                    //  this requires the assumption that a terminal which can't be expanded in a
                    //  given location can never be expanded in that location w/o removing things

                    if (terminal.PassesMissionRequirements(this.Space, sourcePos, destPos))
                    {
                        float cost = terminal.Evaluate(this.Space, sourcePos, destPos);
                        if (cost <= 1.0f)
                        {
                            SpaceCandidate copy = this.DeepCopy();
                            terminal.Expand(copy.Space, sourcePos, destPos);
                            copy.m_mission.MarkTerminal(terminal);

                            successors.Add(copy);

                            childCount++;
                        }
                    }
                }
            }

            if (childCount == 0)
            foreach (IMissionTerminalExpander terminal in m_mission.ReadyTerminals.Where(i => i.LinearReqs.Count == 0))
            {
                foreach (KeyValuePair<CoordPair, Connection> openSlot in (m_space as SpaceImpl).OpenConnectionsRandom)
                {
                    Point sourcePos = openSlot.Key.SourceCoord;
                    Point destPos = openSlot.Key.DestCoord;

                    // TODO
                    // possible optimization: keep hash set of openSlot/terminal pairs
                    //  which have been tried and failed so we don't try them again in successors;
                    //  this requires the assumption that a terminal which can't be expanded in a
                    //  given location can never be expanded in that location w/o removing things

                    if (terminal.PassesMissionRequirements(this.Space, sourcePos, destPos))
                    {
                        float cost = terminal.Evaluate(this.Space, sourcePos, destPos);
                        if (cost <= 1.0f)
                        {
                            SpaceCandidate copy = this.DeepCopy();
                            terminal.Expand(copy.Space, sourcePos, destPos);
                            copy.m_mission.MarkTerminal(terminal);

                            successors.Add(copy);

                            childCount++;
                        }
                    }
                }
            }

            if (childCount == 0)
            {
                // debug me!
                int a = 7;
            }

            return successors;
        }

        public bool IsComplete
        {
            get { return m_mission.Count == 0; }
        }

        #endregion

        protected SpaceCandidate DeepCopy()
        {
            SpaceCandidate copy = new SpaceCandidate();

            copy.m_cost = this.m_cost;
            copy.m_space = this.m_space.DeepCopy();
            copy.m_mission = this.m_mission.DeepCopy();

            // TODO
            // should only be removed once this class is finalized for testing.
            // so we don't have obscure bugs caused by forgetting to copy a new field
            //  which was added later.
            //throw new NotImplementedException();

            return copy;
        }

        public override int GetHashCode()
        {
            return m_mission.GetHashCode() ^ m_space.GetHashCode();
        }
    }
}
