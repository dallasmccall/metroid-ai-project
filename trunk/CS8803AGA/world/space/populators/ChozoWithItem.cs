using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CS8803AGA.controllers;
using CS8803AGA.controllers.mission;

namespace CS8803AGA.world.space.populators
{
    class ChozoWithItem : IObjectPopulator
    {
        protected Item m_item;

        public ChozoWithItem(ParametersTable parameters)
            : this(parameters.ParseEnum<Item>("Item"))
        {
            // nch
        }

        public ChozoWithItem(Item item)
        {
            m_item = item;
        }

        #region IObjectPopulator Members

        public void PopulateObjects(Zone zone, Point globalScreenCoord)
        {
            Vector2 screenOffset = 
                zone.getLocalScreenOffsetInPixelVector(
                    zone.getLocalScreenFromGlobalScreen(globalScreenCoord));
            screenOffset += new Vector2(Zone.SCREEN_WIDTH_IN_PIXELS / 2,
                                        Zone.SCREEN_HEIGHT_IN_PIXELS / 2 + 60);

            zone.add(
                new ChozoStatue(screenOffset, m_item, Direction.Right));
        }

        #endregion
    }
}
