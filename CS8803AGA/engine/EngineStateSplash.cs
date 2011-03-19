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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MetroidAI.audio;

namespace MetroidAI.engine
{
    public class EngineStateSplash : IEngineState
    {
        private const int MIN_FRAMES = 5; // TODO change this back to 45

        private Engine m_engine;
        private GameTexture m_splash;
        private int m_tick = 0;
        private bool m_doneLoading = false;

        public EngineStateSplash(Engine engine)
        {
            m_engine = engine;
            m_splash = new GameTexture(@"Sprites\splash2");
        }

        public void update(GameTime gameTime)
        {
            if (m_tick > 0 && !m_doneLoading)
            {
                FontMap.getInstance().loadFonts("", m_engine.SpriteBatch, m_engine);
                SoundEngine.getInstance();
                m_doneLoading = true;
            }

            if (m_doneLoading && m_tick > MIN_FRAMES)
            {
                EngineManager.replaceCurrentState(new EngineStateStart(m_engine));
                return;
            }

            m_tick++;
        }

        public void draw()
        {
            DrawCommand td = DrawBuffer.getInstance().DrawCommands.pushGet();
            Point p = m_engine.GraphicsDevice.Viewport.TitleSafeArea.Center;
            Vector2 v = new Vector2(p.X, p.Y);
            td.set(m_splash, 0, v, CoordinateTypeEnum.ABSOLUTE, Constants.DepthDebugLines,
                true, Color.White, 0f, 1f);
        }

    }
}
