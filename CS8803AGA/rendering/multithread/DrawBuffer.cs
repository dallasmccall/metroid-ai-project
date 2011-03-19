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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;

namespace CS8803AGA
{
    /// <summary>
    /// Responsible for managing the DrawCommand and FontDrawCommand stacks
    /// for the UpdateThread and RenderThread, and swapping them when the
    /// two threads synchronize (it's a double buffer).
    /// </summary>
    public class DrawBuffer
    {
        private const int FONT_STACK_SIZE = 20;
        private static DrawBuffer s_instance;

        protected DrawStack[] m_drawStacks;
        protected FontStack[] m_fontStacks;
        protected volatile int m_updateBufferIndex;
        protected volatile int m_renderBufferIndex;

        protected AutoResetEvent m_renderFrameStartEvent;
        protected AutoResetEvent m_renderFrameEndEvent;
        protected AutoResetEvent m_updateFrameStartEvent;
        protected AutoResetEvent m_updateFrameEndEvent;

        protected AutoResetEvent m_controlsUpdatedStartEvent;
        protected AutoResetEvent m_controlsUpdatedEndEvent;

        protected volatile GameTime m_gameTime;

        private DrawBuffer(int size, SpriteBatch spriteBatch)
        {
            m_drawStacks = new DrawStack[2];
            m_drawStacks[0] = new DrawStack(size);
            m_drawStacks[1] = new DrawStack(size);
            m_fontStacks = new FontStack[2];
            m_fontStacks[0] = new FontStack(FONT_STACK_SIZE, spriteBatch);
            m_fontStacks[1] = new FontStack(FONT_STACK_SIZE, spriteBatch);

            m_renderFrameStartEvent = new AutoResetEvent(false);
            m_renderFrameEndEvent = new AutoResetEvent(false);
            m_updateFrameStartEvent = new AutoResetEvent(false);
            m_updateFrameEndEvent = new AutoResetEvent(false);
            m_controlsUpdatedStartEvent = new AutoResetEvent(false);
            m_controlsUpdatedEndEvent = new AutoResetEvent(false);
        }

        public static DrawBuffer getInstance()
        {
            if (s_instance == null)
            {
                throw new Exception("DrawBuffer must be initialized before it can be used!");
            }
            return s_instance;
        }

        public static void initialize(int size, SpriteBatch spriteBatch)
        {
            s_instance = new DrawBuffer(size, spriteBatch);
        }

        /// <summary>
        /// Resizes the buffers in a destructive manner
        /// </summary>
        /// <param name="size">The new size of each buffer</param>
        public void resizeTheBuffers(int size)
        {
            m_drawStacks[0].resizeDestructively(size);
            m_drawStacks[1].resizeDestructively(size);
        }

        public void reset()
        {
            //reset the buffer indices
            m_updateBufferIndex = 0;
            m_renderBufferIndex = 1;

            //set all events to non-signaled
            m_renderFrameStartEvent.Reset();
            m_renderFrameEndEvent.Reset();
            m_updateFrameStartEvent.Reset();
            m_updateFrameEndEvent.Reset();
            m_controlsUpdatedEndEvent.Reset();
            m_controlsUpdatedStartEvent.Reset();
        }

        public void cleanUp()
        {
            //release system resources
            m_renderFrameStartEvent.Close();
            m_renderFrameEndEvent.Close();
            m_updateFrameStartEvent.Close();
            m_updateFrameEndEvent.Close();
            m_controlsUpdatedEndEvent.Close();
            m_controlsUpdatedStartEvent.Close();
        }

        private void swapBuffers()
        {
            m_renderBufferIndex = m_updateBufferIndex;
            m_updateBufferIndex = (m_updateBufferIndex + 1) % 2;
        }

        public void globalStartControlInput(GameTime gameTime)
        {
            m_controlsUpdatedStartEvent.Set();
        }

        public void globalSynchronizeControlInput()
        {
            m_controlsUpdatedEndEvent.WaitOne();
            Thread.MemoryBarrier();
        }

        public void startControlsProcessing()
        {
            m_controlsUpdatedStartEvent.WaitOne();
        }

        public void submitControls()
        {
            m_controlsUpdatedEndEvent.Set();
            Thread.MemoryBarrier();
        }

        public void globalStartFrame(GameTime gameTime)
        {
            swapBuffers();

            //signal the render and update threads to start processing
            m_renderFrameStartEvent.Set();
            m_updateFrameStartEvent.Set();

            this.m_gameTime = gameTime;
        }
        public void globalSynchronize()
        {
            //wait until both threads signal that they are finished
            m_renderFrameEndEvent.WaitOne();
            m_updateFrameEndEvent.WaitOne();
        }

        public void startUpdateProcessing(out GameTime gameTime)
        {
            //wait for start signal
            m_updateFrameStartEvent.WaitOne();
            //ensure cache coherency
            Thread.MemoryBarrier();
            gameTime = m_gameTime;
        }

        public void startRenderProcessing(out GameTime gameTime)
        {
            //wait for start signal
            m_renderFrameStartEvent.WaitOne();
            //ensure cache coherency
            Thread.MemoryBarrier();
            gameTime = m_gameTime;
        }

        public void submitUpdate()
        {
            //update is done
            m_updateFrameEndEvent.Set();
            //ensure cache coherency
            Thread.MemoryBarrier();
        }
        
        public void submitRender()
        {
            //render is done
            m_renderFrameEndEvent.Set();
            //ensure cache coherency
            Thread.MemoryBarrier();
        }

        /// <summary>
        /// Gets the stack which game logic should use to send commands to
        /// render images.
        /// </summary>
        public DrawStack DrawCommands
        {
            get { return m_drawStacks[m_updateBufferIndex]; }
        }

        /// <summary>
        /// ENGINE USE ONLY
        /// Gets the stack whose DrawCommands should be executed.
        /// </summary>
        /// <returns></returns>
        public DrawStack getRenderStack()
        {
            return m_drawStacks[m_renderBufferIndex];
        }

        /// <summary>
        /// Gets the stack which game logic should use to send commands to
        /// render text.
        /// </summary>
        public FontStack FontDrawCommands
        {
            get { return m_fontStacks[m_updateBufferIndex]; }
        }

        /// <summary>
        /// ENGINE USE ONLY
        /// Gets the stack whose FontDrawCommands should be executed.
        /// </summary>
        /// <returns></returns>
        public FontStack getRenderFontStack()
        {
            return m_fontStacks[m_renderBufferIndex];
        }
    }
}
