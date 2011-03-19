using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS8803AGA.world.space
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
