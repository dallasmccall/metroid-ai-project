using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS8803AGA.world.space
{
    class MissionQueueDummy : IMissionQueue
    {
        protected List<IMissionTerminalExpander> m_terminals = new List<IMissionTerminalExpander>();

        #region IMissionQueue Members

        public int Count
        {
            get { return m_terminals.Count; }
        }

        public List<IMissionTerminalExpander> ReadyTerminals
        {
            get { return m_terminals; }
        }

        public IMissionQueue DeepCopy()
        {
            MissionQueueDummy copy = new MissionQueueDummy();
            copy.m_terminals = new List<IMissionTerminalExpander>(m_terminals);
            return copy;
        }

        public void MarkTerminal(IMissionTerminalExpander terminal)
        {
            m_terminals.Remove(terminal);
        }

        #endregion

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
