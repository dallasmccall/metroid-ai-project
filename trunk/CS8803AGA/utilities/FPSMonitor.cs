/*
 ***************************************************************************
 * Copyright notice removed by a creator for anonymity, please don't sue   *
 *                                                                         *
 * Licensed under the Apache License, Version 2.0 (the "License");         *
 * you may not use this file except in compliance with the License.        *
 * You may obtain a copy of the License at                                 *
 *                                                                         *
 * http://www.apache.org/licenses/LICENSE-2.0                              *
 *                                                                         *
 * Unless required by applicable law or agreed to in writing, software     *
 * distributed under the License is distributed on an "AS IS" BASIS,       *
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.*
 * See the License for the specific language governing permissions and     *
 * limitations under the License.                                          *
 ***************************************************************************
*/

// ATTENTION!
// The code of this class is derived from
//  http://blogs.msdn.com/shawnhar/archive/2007/06/08/displaying-the-framerate.aspx
// Thanks Shawn Hargreaves!

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS8803AGA.utilties
{
    /// <summary>
    /// Displays the current frame rate of the game.
    /// </summary>
    internal class FPSMonitor
    {
        private static FPSMonitor s_instance;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;
        GameFont font;

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        private FPSMonitor()
        {
            font = FontMap.getInstance().getFont(FontEnum.Kootenay14);
        }

        /// <summary>
        /// Singleton pattern to delay instantiation.
        /// </summary>
        /// <returns>Returns the singleton instance.</returns>
        internal static FPSMonitor getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new FPSMonitor();
            }
            return s_instance;
        }

        /// <summary>
        /// Updates the framerate calculation.
        /// </summary>
        /// <param name="gameTime">Gametime parameter.</param>
        internal void update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        /// <summary>
        /// Display framerate calculation to the screen.
        /// </summary>
        /// <param name="gameTime">Gametime parameter.</param>
        internal void draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);

            font.drawString(fps, new Vector2(33, 33), Color.Black, 0, Vector2.Zero, 1f, SpriteEffects.None, Constants.DepthDebugLines);
            font.drawString(fps, new Vector2(33, 32), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, Constants.DepthDebugLines);
        }

    }
}
