using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.world.space;

namespace CS8803AGA.world.mission
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
