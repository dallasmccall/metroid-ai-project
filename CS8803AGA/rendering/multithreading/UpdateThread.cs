/*
***************************************************************************
* Copyright 2009 Eric Barnes, Ken Hartsook, Andrew Pitman, & Jared Segal  *
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

namespace QuestAdaptation
{
    public class UpdateThread
    {
        public Thread RunningThread { get; set; }

        public ControllerInputInterface Controls_ { get; set; }

        protected EngineStateInterface currentEngineState_;

        protected Engine engine_;

        protected DrawBuffer drawBuffer_;

        protected GameTime gameTime_;

        public UpdateThread(Engine engine, EngineStateInterface engineState)
        {
            currentEngineState_ = engineState;
            engine_ = engine;
            drawBuffer_ = DrawBuffer.getInstance();
        }

        public void tick()
        {
            drawBuffer_.startUpdateProcessing(out gameTime_);
            update();
            drawBuffer_.submitUpdate();
        }

        public void run()
        {
        #if XBOX
            Thread.CurrentThread.SetProcessorAffinity(5);
        #endif

            while (true)
            {
                try
                {
                    tick();
                }
                catch (Exception e)
                {
                    throw e;
                }
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
            if (!engine_.IsActive && !(currentEngineState_ is EngineStateOutofFocus)
#if !XBOX
 || mouseOutsideWindow() && !(currentEngineState_ is EngineStateOutofFocus)
#endif
                )
            {
                currentEngineState_ = new EngineStateOutofFocus(engine_, currentEngineState_);
            }

            //if (Controls_ != null)
            //    Controls_.updateInputSet();
            //drawBuffer_.globalSynchronizeControlInput();

            currentEngineState_ = currentEngineState_.update(gameTime_);

            currentEngineState_.draw();
            Camera cam = GlobalHelper.getInstance().getCurrentCamera();
            if (cam != null)
            {
                drawBuffer_.getUpdateStack().getCamera().setPosition(cam.getX(), cam.getY());
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
                ms.X > engine_.GraphicsDevice.Viewport.X + engine_.GraphicsDevice.Viewport.Width ||
                ms.Y > engine_.GraphicsDevice.Viewport.Y + engine_.GraphicsDevice.Viewport.Height)
            {
                return true;
            }
            return false;
        }
#endif
    }
}
