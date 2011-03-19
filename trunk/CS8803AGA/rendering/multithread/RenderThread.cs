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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CS8803AGA
{
    /// <summary>
    /// Thread responsible for popping commands off of the draw command stacks
    /// and rendering them.
    /// </summary>
    public class RenderThread
    {
        protected DrawBuffer m_drawBuffer;

        protected GameTime m_gameTime;

        public RenderThread()
        {
            m_drawBuffer = DrawBuffer.getInstance();
        }

        public void tick()
        {
            //m_spriteBatch = GlobalHelper.getInstance().getEngine().m_spriteBatch;

            m_drawBuffer.startRenderProcessing(out m_gameTime);
            draw();
            m_drawBuffer.submitRender();
        }

        public void draw()
        {
            DrawStack renderStack = m_drawBuffer.getRenderStack();
            FontStack fontStack = m_drawBuffer.getRenderFontStack();
            Vector2 camPosition = renderStack.Camera.Position;
            
            while (renderStack.hasMoreItems())
            {
                renderStack.pop().draw(camPosition);
            }

            while (fontStack.hasMoreItems())
            {
                fontStack.pop().draw(camPosition);
            }
        }
    }
}
