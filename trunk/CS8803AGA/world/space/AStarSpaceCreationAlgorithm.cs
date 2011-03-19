using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mischel.Collections;

namespace MetroidAI.world.space
{
    class AStarSpaceCreationAlgorithm : ISpaceCreationAlgorithm
    {
        public delegate float Heuristic(ISpaceCandidate candidate);

        protected IMissionQueue m_mission;
        protected Heuristic m_heuristic;

        public AStarSpaceCreationAlgorithm(Heuristic h)
        {
            m_heuristic = h;
        }

        #region ISpaceCreationAlgorithm Members

        public IMissionQueue Mission
        {
            set
            {
                m_mission = value;
            }
        }

        public ISpace ConstructSpace()
        {
            PriorityQueue<ISpaceCandidate, float> openList = new PriorityQueue<ISpaceCandidate, float>();
            HashSet<int> closedList = new HashSet<int>();

            SpaceCandidate initialCandidate = new SpaceCandidate(m_mission);

            openList.Enqueue(initialCandidate, 0.0f);

            while (openList.Count > 0)
            {
                ISpaceCandidate curNode = openList.Dequeue().Value;

                // closedList simply checks the hash.  yes, its possible that then we'll get a false
                //  positive later because two things hash to the same value.  i dont want to impl
                //  a more rigorous Equals function and use a dict, so i'm hoping that these false
                //  positives are minimal given the 4 billion possible hash values.
                if (closedList.Contains(curNode.GetHashCode()))
                {
                    continue;
                }
                closedList.Add(curNode.GetHashCode());

                if (curNode.IsComplete)
                {
                    return curNode.Space;
                }

                foreach (ISpaceCandidate successor in curNode.Expand())
                {
                    float cost = successor.Cost;
                    float heuristic = m_heuristic(successor);

                    openList.Enqueue(successor, 1000000 - (cost + heuristic));
                }
            }

            return null;
        }

        #endregion

        public static float BestFirstSearch(ISpaceCandidate candidate)
        {
            return 0.0f;
        }

        public static float Suboptimal(ISpaceCandidate candidate)
        {
            return candidate.MissionQueue.Count * 0.5f;
        }

        public static float RandomDFS(ISpaceCandidate candidate)
        {
            s_randomDFSCount--;
            return s_randomDFSCount + RandomManager.get().Next(100);
        }
        static int s_randomDFSCount = int.MaxValue/2;
    }
}
