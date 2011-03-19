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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MetroidAI
{
    /// <summary>
    /// Loaded image data and location of sprites on a sprite sheet.
    /// </summary>
    public class GameTexture
    {
        #region Static

        internal static SpriteBatch s_spriteBatch { get; set; }

        internal static ContentManager s_content { get; set; }

        #endregion

        /// <summary>
        /// Image file loaded into memory
        /// 
        /// Do NOT change this property so that it can be set after creation.
        /// Bad multithreaded things can happen if it is ever changed.
        /// </summary>
        internal Texture2D Texture { get; private set; }

        /// <summary>
        /// Rectangular areas representing areas on the texture which should
        /// be individually drawn (e.g. sprites on a sprite sheet)
        /// 
        /// Do NOT change this property so that it can be set after creation.
        /// Bad multithreaded things can happen if it is ever changed.
        /// </summary>
        internal Rectangle[] ImageDimensions { get; private set; }

        /// <summary>
        /// Basic constructor for a GameTexture, loads the image from file
        /// and sets ImageDimensions[0] to the full size of the image file
        /// </summary>
        /// <param name="assetName">Path to image in Content</param>
        public GameTexture(string assetName)
        {
            Texture = s_content.Load<Texture2D>(assetName);

            ImageDimensions = new Rectangle[1];
            ImageDimensions[0] = new Rectangle(0, 0, Texture.Width, Texture.Height);
        }

        /// <summary>
        /// Loads image from file, and sets ImageDimensions to the provided
        /// values
        /// </summary>
        /// <param name="assetName">Path to image in Content</param>
        /// <param name="imageDimensions">Locations of sprites on image</param>
        public GameTexture(string assetName, Rectangle[] imageDimensions)
        {
            Texture = s_content.Load<Texture2D>(assetName);

            ImageDimensions = imageDimensions;
        }
    }
}
