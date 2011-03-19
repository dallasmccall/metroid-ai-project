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

/*
 * This code borrows heavily from a tutorial on MultiThreaded XNA programming
 * by Catalin Zima found here 
 *                    http://www.ziggyware.com/readarticle.php?article_id=221
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using CS8803AGA.devices;
using CS8803AGA.engine;

namespace CS8803AGA
{
    /// <summary>
    /// Thread responsible for updating game logic and filling the draw stacks
    /// with rendering commands for the RenderThread.
    /// </summary>
    public class UpdateThread
    {
        public Thread RunningThread { get; set; }

        public ControllerInputInterface Controls { get; set; }

        protected Engine m_engine;

        protected DrawBuffer m_drawBuffer;

        protected GameTime m_gameTime;

        public UpdateThread(Engine engine)
        {
            m_engine = engine;
            m_drawBuffer = DrawBuffer.getInstance();
        }

        public void tick()
        {
            m_drawBuffer.startUpdateProcessing(out m_gameTime);
            update();
            m_drawBuffer.submitUpdate();
        }

        public void run()
        {
        #if XBOX
            Thread.CurrentThread.SetProcessorAffinity(5);
        #endif

            while (true)
            {
                tick();
            }
        }

        public void startThread()
        {
            ThreadStart ts = new ThreadStart(run);
            RunningThread = new Thread(ts);
            RunningThread.Start();
        }

        public void update()
        {
            // TODO: Add your update logic here
            /*
            if (!engine_.IsActive && !(currentEngineState_ is EngineStateOutofFocus)
#if !XBOX
 || mouseOutsideWindow() && !(currentEngineState_ is EngineStateOutofFocus)
#endif
                )
            {
                currentEngineState_ = new EngineStateOutofFocus(engine_, currentEngineState_);
            }
            */

            //if (Controls_ != null)
            //    Controls_.updateInputSet();
            //drawBuffer_.globalSynchronizeControlInput();

            EngineManager.peekAtState().update(m_gameTime);
            EngineManager.peekAtState().draw();

            Camera cam = GlobalHelper.getInstance().Camera;
            if (cam != null)
            {
                m_drawBuffer.DrawCommands.Camera.Position = cam.Position;
            }
        }

#if !XBOX
        /// <summary>
        /// Whether or not the mouse is outside the game window.
        /// </summary>
        /// <returns>True is the mouse is outside the game window, false otherwise</returns>
        internal protected bool mouseOutsideWindow()
        {
            MouseState ms = Mouse.GetState();
            if (ms.X < 0 || ms.Y < 0 ||
                ms.X > m_engine.GraphicsDevice.Viewport.X + m_engine.GraphicsDevice.Viewport.Width ||
                ms.Y > m_engine.GraphicsDevice.Viewport.Y + m_engine.GraphicsDevice.Viewport.Height)
            {
                return true;
            }
            return false;
        }
#endif
    }
}
