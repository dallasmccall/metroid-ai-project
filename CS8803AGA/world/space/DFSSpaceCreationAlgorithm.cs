using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidAI.world.space
{
    class DFSSpaceCreationAlgorithm : ISpaceCreationAlgorithm
    {
        protected IMissionQueue m_mission;

        #region ISpaceCreationAlgorithm Members

        public IMissionQueue Mission
        {
            set { m_mission = value; }
        }

        public ISpace ConstructSpace()
        {
            Stack<ISpaceCandidate> stack = new Stack<ISpaceCandidate>();
            HashSet<int> closedList = new HashSet<int>();
            //Dictionary<int, ISpaceCandidate> debugClosedList = new Dictionary<int, ISpaceCandidate>();

            int nodesExplored = 0;
            int dupesRemoved = 0;

            SpaceCandidate initialCandidate = new SpaceCandidate(m_mission);

            stack.Push(initialCandidate);

            while (stack.Count > 0)
            {
                ISpaceCandidate curNode = stack.Pop();
                nodesExplored++;

                if (curNode.IsComplete)
                {
                    return curNode.Space;
                }

                List<ISpaceCandidate> successors = curNode.Expand();
                foreach (ISpaceCandidate successor in successors)
                {
                    /*
                     * Tests if we ever have a hash collision when spaces aren't identical.
                     * I tried a couple dozen times and never had this occur.
                    if (closedList.Contains(curNode.GetHashCode()) &&
                        !SpaceImpl.TestAreEqual((SpaceImpl)curNode.Space, (SpaceImpl)debugClosedList[curNode.GetHashCode()].Space))
                    {
                        // debug me!
                        SpaceImpl a = (SpaceImpl)curNode.Space;
                        SpaceImpl b = (SpaceImpl)debugClosedList[curNode.GetHashCode()].Space;
                        int c = 7;
                    }
                     */

                    if (!closedList.Contains(successor.GetHashCode()))
                    {
                        closedList.Add(successor.GetHashCode());
                        //debugClosedList.Add(successor.GetHashCode(), curNode);
                        stack.Push(successor);
                    }
                    else
                    {
                        dupesRemoved++;
                    }
                }
            }

            return null;
        }

        #endregion
    }
}
