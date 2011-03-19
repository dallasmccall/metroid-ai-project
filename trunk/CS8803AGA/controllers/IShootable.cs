using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetroidAI.controllers
{
    interface IShootable
    {
        void handleProjectileHit(ProjectileController projectile);
    }
}
