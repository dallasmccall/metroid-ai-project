using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI.controllers.mission
{
    class RefillEnergy : ARefillController
    {
        public RefillEnergy(Vector2 position)
            : base(position, "Energy")
        {
            // nch
        }

        protected override bool refill(PlayerController samus)
        {
            if (samus.Health == samus.MaxHealth)
            {
                return false;
            }
            samus.Health = samus.MaxHealth;
            return true;
        }
    }
}
