using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidAI.world.space
{
    /// <summary>
    /// Algorithm for mapping a Mission into a Space.
    /// </summary>
    interface ISpaceCreationAlgorithm
    {
        IMissionQueue Mission { set; }

        ISpace ConstructSpace();
    }
}
