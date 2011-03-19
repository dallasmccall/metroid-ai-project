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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace QuestAdaptation
{
    public class RenderThread
    {
        protected DrawBuffer drawBuffer_;

        protected SpriteBatch spriteBatch_;

        protected GameTime gameTime_;

        public RenderThread()
        {
            drawBuffer_ = DrawBuffer.getInstance();
        }

        public void tick()
        {
            drawBuffer_.startRenderProcessing(out gameTime_);
            draw();
            drawBuffer_.submitRender();
        }

        public void draw()
        {
            DrawStack renderStack = drawBuffer_.getRenderStack();
            FontStack fontStack = drawBuffer_.getRenderFontStack();
            Vector2 camPos = renderStack.getCamera().getPosition();

            //spriteBatch_.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.FrontToBack, SaveStateMode.None);
            
            while (renderStack.hasMoreItems())
            {
                renderStack.pop().draw(camPos);
            }

            while (fontStack.hasMoreItems())
            {
                fontStack.pop().draw();
            }

            //spriteBatch_.End();
        }
    }
}
