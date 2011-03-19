using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MetroidAI.world.space
{
    interface IObjectPopulator
    {
        void PopulateObjects(Zone zone, Point globalScreenCoord);
    }

    class ObjectPopulatorEmpty : IObjectPopulator
    {
        public void PopulateObjects(Zone zone, Point globalScreenCoord)
        {
            // nch
        }
    }
}
