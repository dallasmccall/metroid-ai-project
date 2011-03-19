using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using MetroidAI.controllers.enemies;

namespace MetroidAI.world.space.populators
{
    class SingleRipper : IObjectPopulator
    {
        #region IObjectPopulator Members

        public void PopulateObjects(Zone zone, Point globalScreenCoord)
        {
            Point offset = zone.getLocalScreenOffsetInPixels(
                zone.getLocalScreenFromGlobalScreen(globalScreenCoord));

            Vector2 ripperPos = new Vector2(
                offset.X + Zone.SCREEN_WIDTH_IN_PIXELS / 2,
                offset.Y + Zone.SCREEN_HEIGHT_IN_PIXELS / 2);

            zone.add(
                new RipperController(ripperPos));
        }

        #endregion
    }
}
