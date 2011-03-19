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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace CS8803AGA
{
    /// <summary>
    /// Encapsulates SpriteFonts to make them easier to use
    /// </summary>
    public class GameFont
    {
        /// <summary>
        /// The low-level font object
        /// </summary>
        protected SpriteFont m_font;

        /// <summary>
        /// A SpriteBatch is necessary to draw the font
        /// </summary>
        protected SpriteBatch m_spriteBatch;

        /// <summary>
        /// The private constructor prevents anyone from using the default constructor
        /// </summary>
        private GameFont()
        {

        }

        /// <summary>
        /// This is the constructor that should almost always be used
        /// </summary>
        /// <param name="filepath">The path within Content to the .spritefont file, without .spritefont at the end</param>
        /// <param name="spriteBatch">The spriteBatch to be used when drawing text</param>
        /// <param name="engine">The main Engine class</param>
        public GameFont(string filename, SpriteBatch spriteBatch, Engine engine)
        {
            m_spriteBatch = spriteBatch;
            m_font = engine.Content.Load<SpriteFont>(filename);
        }

        /// <summary>
        /// Public accessor method
        /// </summary>
        /// <returns>Returns the current SpriteFont</returns>
        public SpriteFont getFont()
        {
            return m_font;
        }

        /// <summary>
        /// Public setter method
        /// </summary>
        /// <param name="sFont">The SpriteFont you want the GameFont to use</param>
        public void setFont(SpriteFont sFont)
        {
            m_font = sFont;
        }

        /// <summary>
        /// Public accessor method
        /// </summary>
        /// <returns>Returns the SpriteBatch used for drawing strings</returns>
        public SpriteBatch getSpriteBatch()
        {
            return m_spriteBatch;
        }

        /// <summary>
        /// Public setter method
        /// </summary>
        /// <param name="sBatch">The SpriteBatch you want the GameFont to use</param>
        public void setSpriteBatch(SpriteBatch sBatch)
        {
            m_spriteBatch = sBatch;
        }

        /// <summary>
        /// Draws a string to the screen with the specified options
        /// </summary>
        /// <param name="text">The text you want to draw</param>
        /// <param name="pos">The position to start drawing the top-left corner of the text</param>
        /// <param name="color">The color of the drawn text</param>
        public void drawString(string text, Vector2 pos, Color color)
        {
            //m_spriteBatch.DrawString(m_font, text, pos, color);
            FontStack stack = DrawBuffer.getInstance().FontDrawCommands;
            FontDrawCommand fd = stack.pushGet();
            fd.set(m_font,
                    text,
                    pos,
                    color,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    1.0f);
        }

        /// <summary>
        /// Draws a string to the screen with the specified options
        /// </summary>
        /// <param name="text">The text you want to draw</param>
        /// <param name="pos">The position to start drawing the top-left corner of the text</param>
        /// <param name="color">The color of the drawn text</param>
        internal void drawString(string text, Vector2 pos, Color color, CoordinateTypeEnum coordType)
        {
            FontStack stack = DrawBuffer.getInstance().FontDrawCommands;
            FontDrawCommand fd = stack.pushGet();
            fd.set(m_font,
                    text,
                    pos,
                    color,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    1.0f);
            fd.CoordinateType = coordType;
        }

        /// <summary>
        /// Draws a string to the screen with the specified options
        /// </summary>
        /// <param name="text">The text you want to draw</param>
        /// <param name="pos">The position to start drawing the top-left corner of the text</param>
        /// <param name="color">The color of the drawn text</param>
        /// <param name="rotation">How far to rotate the text</param>
        /// <param name="origin">The tileWidth and tileHeight of the text</param>
        /// <param name="scale">How big or small to scale the text</param>
        /// <param name="effects">Which SpriteEffects to add to the text</param>
        /// <param name="layerDepth">The depth at which to draw the text</param>
        public void drawString(string text, Vector2 pos, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            //m_spriteBatch.DrawString(m_font, text, pos, color, rotation, origin, scale, effects, layerDepth);
            FontStack stack = DrawBuffer.getInstance().FontDrawCommands;
            FontDrawCommand fd = stack.pushGet();
            fd.set(m_font,
                    text,
                    pos,
                    color,
                    rotation,
                    origin,
                    scale,
                    effects,
                    layerDepth);
        }

        /// <summary>
        /// Draws a string to the screen with the specified options
        /// </summary>
        /// <param name="text">The text you want to draw</param>
        /// <param name="pos">The position to start drawing the top-left corner of the text</param>
        /// <param name="color">The color of the drawn text</param>
        /// <param name="rotation">How far to rotate the text</param>
        /// <param name="layerDepth">The depth at which to draw the text</param>
        public void drawStringCentered(string text, Vector2 pos, Color color, float rotation, float layerDepth)
        {
            Vector2 origin = m_font.MeasureString(text);
            origin.X /= 2f;
            origin.Y /= 2f;
            //m_spriteBatch.DrawString(m_font, text, pos, color, rotation, new Vector2(origin.X / 2, origin.Y / 2), 1.0f, SpriteEffects.None, layerDepth);
            FontStack stack = DrawBuffer.getInstance().FontDrawCommands;
            FontDrawCommand fd = stack.pushGet();
            fd.set(m_font,
                    text,
                    pos,
                    color,
                    rotation,
                    origin,
                    1.0f,
                    SpriteEffects.None,
                    layerDepth);
        }

        /// <summary>
        /// Draws a string to the screen with the specified options
        /// </summary>
        /// <param name="text">The text you want to draw</param>
        /// <param name="pos">The position to start drawing the top-left corner of the text</param>
        /// <param name="color">The color of the drawn text</param>
        /// <param name="rotation">How far to rotate the text</param>
        /// <param name="scale">How big or small to scale the text</param>
        /// <param name="effects">Which SpriteEffects to add to the text</param>
        /// <param name="layerDepth">The depth at which to draw the text</param>
        public void drawStringCentered(string text, Vector2 pos, Color color, float rotation, float scale, SpriteEffects effects, float layerDepth)
        {
            Vector2 origin = m_font.MeasureString(text);
            origin.X /= 2f;
            origin.Y /= 2f;
            //m_spriteBatch.DrawString(m_font, text, pos, color, rotation, new Vector2(origin.X / 2, origin.Y / 2), scale, effects, layerDepth);
            FontStack stack = DrawBuffer.getInstance().FontDrawCommands;
            FontDrawCommand fd = stack.pushGet();
            fd.set(m_font,
                    text,
                    pos,
                    color,
                    rotation,
                    origin,
                    scale,
                    effects,
                    layerDepth);
        }
    }
}
