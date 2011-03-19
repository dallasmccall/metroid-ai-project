using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.world.space
{
    public interface IScreenPostProcessor
    {
        void PostProcess(Zone zone, Point globalScreenCoord);
    }

    public class ScreenPostProcessorEmpty : IScreenPostProcessor
    {
        public void PostProcess(Zone zone, Point globalScreenCoord)
        {
            // nch
        }
    }
}
