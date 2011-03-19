using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidAI.world.space
{
    /// <summary>
    /// Container for a graph of mission terminals during a mapping onto a
    /// space.  Tracks which terminals have been mapped and are ready to be.
    /// </summary>
    interface IMissionQueue
    {
        int Count { get; }

        List<IMissionTerminalExpander> ReadyTerminals { get; }

        IMissionQueue DeepCopy();

        void MarkTerminal(IMissionTerminalExpander terminal);
    }
}
