using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CS8803AGA.collision;

namespace CS8803AGA.controllers
{
    public interface ITrigger : ICollidable
    {
        void handleImpact(Collider mover);
    }
}
