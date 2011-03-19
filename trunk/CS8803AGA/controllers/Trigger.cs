using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MetroidAI.collision;

namespace MetroidAI.controllers
{
    public interface ITrigger : ICollidable
    {
        void handleImpact(Collider mover);
    }
}
