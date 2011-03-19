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

namespace MetroidAI
{
    /// <summary>
    /// Singleton that stores GameFonts and allows text to be drawn to the screen
    /// </summary>
    class FontMap
    {
        /// <summary>
        /// The Dictionary (or Map) of FontEnums to GameFonts
        /// </summary>
        protected Dictionary<FontEnum, GameFont> m_fonts;

        /// <summary>
        /// The instance data member to implement a Singleton class
        /// </summary>
        protected static FontMap s_instance = null;

        /// <summary>
        /// The constructor
        /// </summary>
        private FontMap()
        {
            m_fonts = new Dictionary<FontEnum, GameFont>();
        }

        /// <summary>
        /// Gets an instance of the Singleton FontMap
        /// </summary>
        /// <returns>Returns the instance to use</returns>
        public static FontMap getInstance()
        {
            if (s_instance == null)
            {
                s_instance = new FontMap();
            }
            return s_instance;
        }

        /// <summary>
        /// Loads SpriteFont information from file
        /// </summary>
        /// <param name="filepath">The .xml file that contains all the Font information</param>
        /// <param name="spriteBatch">The spriteBatch that will be used to draw text</param>
        /// <param name="engine">The main Engine class</param>
        public void loadFonts(string filename, SpriteBatch spriteBatch, Engine engine)
        {
            //TODO: Eventually, create automatic loading of fonts based on an xml file.
            //      For now, just create the load for each font in this function
            m_fonts.Add(FontEnum.Consolas16, new GameFont("SpriteFonts/Consolas16", spriteBatch, engine));
            m_fonts.Add(FontEnum.Kootenay8, new GameFont("SpriteFonts/Kootenay8", spriteBatch, engine));
            m_fonts.Add(FontEnum.Kootenay14, new GameFont("SpriteFonts/Kootenay", spriteBatch, engine));
            m_fonts.Add(FontEnum.Kootenay48, new GameFont("SpriteFonts/Kootenay48", spriteBatch, engine));
            m_fonts.Add(FontEnum.Lindsey, new GameFont("SpriteFonts/Lindsey", spriteBatch, engine));
            m_fonts.Add(FontEnum.Miramonte, new GameFont("SpriteFonts/Miramonte", spriteBatch, engine));
            m_fonts.Add(FontEnum.MiramonteBold, new GameFont("SpriteFonts/MiramonteBold", spriteBatch, engine));
            m_fonts.Add(FontEnum.Pericles, new GameFont("SpriteFonts/Pericles", spriteBatch, engine));
            m_fonts.Add(FontEnum.PericlesLight, new GameFont("SpriteFonts/PericlesLight", spriteBatch, engine));
            m_fonts.Add(FontEnum.Pescadero, new GameFont("SpriteFonts/Pescadero", spriteBatch, engine));
            m_fonts.Add(FontEnum.PescaderoBold, new GameFont("SpriteFonts/PescaderoBold", spriteBatch, engine));
        }

        /// <summary>
        /// Gets the GameFont used for drawing text
        /// </summary>
        /// <param name="fontName">The font you want to draw</param>
        /// <returns>The GameFont used for actually drawing the text</returns>
        public GameFont getFont(FontEnum fontName)
        {
            if (!m_fonts.ContainsKey(fontName))
            {
                return null;
            }
            return m_fonts[fontName];
        }
    }
}
