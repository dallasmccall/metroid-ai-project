using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.world.space;

namespace MetroidAI.world.mission
{
    /// <summary>
    /// Encapsulation of a mission graph.
    /// Needs to be able to return an IMissionQueue so that an
    /// ISpaceCreationAlgorithm can use it to, well, create a space.
    /// </summary>
    interface IMission
    {
        IMissionQueue MissionQueue { get; }
    }
}
