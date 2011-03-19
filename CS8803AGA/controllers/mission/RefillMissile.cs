using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI.controllers.mission
{
    class RefillMissile : ARefillController
    {
        public RefillMissile(Vector2 position)
            : base(position, "Missile")
        {
            // nch
        }

        protected override bool refill(PlayerController samus)
        {
            if (samus.MissileCount == samus.MaxMissileCount)
            {
                return false;
            }
            samus.MissileCount = samus.MaxMissileCount;
            return true;
        }
    }
}
