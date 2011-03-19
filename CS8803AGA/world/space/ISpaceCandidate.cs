using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidAI.world.space
{
    /// <summary>
    /// Interface for a step in the process of mapping a Mission to a Space.
    /// Used for state-space type searches.
    /// </summary>
    interface ISpaceCandidate
    {
        ISpace Space { get; }
        IMissionQueue MissionQueue { get; }

        float Cost { get; }

        bool IsComplete { get; }

        List<ISpaceCandidate> Expand();
    }
}
