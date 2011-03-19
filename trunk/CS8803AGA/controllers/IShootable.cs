using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS8803AGA.controllers
{
    interface IShootable
    {
        void handleProjectileHit(ProjectileController projectile);
    }
}
