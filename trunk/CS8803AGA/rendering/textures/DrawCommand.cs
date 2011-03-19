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
    /// Whether the coordinates within a DrawCommand are Absolute or
    /// Relative to the Camera.
    /// </summary>
    internal enum CoordinateTypeEnum
    {
        RELATIVE,
        ABSOLUTE
    }

    /// <summary>
    /// Encapsulates a command to XNA to draw a portion of a GameTexture
    /// on a certain area of the screen in a certain way.
    /// </summary>
    internal class DrawCommand
    {
        /// <summary>
        /// Image data to draw.
        /// </summary>
        internal GameTexture Texture { get; set; }

        /// <summary>
        /// Which ImageDimensions index to access from the GameTexture;
        /// i.e. index of which Rectangle in the image data to draw
        /// </summary>
        internal int ImageIndex { get; set; }

        /// <summary>
        /// Where on the screen the image should be drawn.
        /// </summary>
        internal Vector2 Position { get; set; }

        /// <summary>
        /// A Destination Rectangle to which to draw the image,
        /// overrides Position.
        /// </summary>
        internal Rectangle Destination { get; set; }

        /// <summary>
        /// True if a Destination has been set and should be used.
        /// </summary>
        internal bool UseDestination { get; set; }

        /// <summary>
        /// Rotation on the image to be drawn, in radians.
        /// </summary>
        internal float Rotation { get; set; }

        /// <summary>
        /// Rotation on the image to be drawn, in vector form.
        /// </summary>
        internal Vector2 Direction
        {
            get
            {
                return CommonFunctions.getVector(Rotation);
            }

            set
            {
                Rotation = CommonFunctions.getAngle(value);
            }
        }

        /// <summary>
        /// Whether the Position is Absolute or Relative to the Camera.
        /// </summary>
        internal CoordinateTypeEnum CoordinateType { get; set; }

        /// <summary>
        /// Render depth for sort order; 
        /// </summary>
        internal float Depth { get; set; }

        /// <summary>
        /// Color to apply to the image.
        /// </summary>
        internal Color Color { get; set; }

        /// <summary>
        /// SpriteEffects to apply to the image.
        /// </summary>
        internal SpriteEffects Effects { get; set; }

        /// <summary>
        /// True if the image should be centered on the provided Position,
        /// False if the image should be drawn with the upper-left there.
        /// </summary>
        internal bool Centered { get; set; }

        /// <summary>
        /// Scale at which the image should be drawn (1.0 is true size).
        /// </summary>
        internal float Scale { get; set; }

        /// <summary>
        /// Default ctor for a DrawCommand, you need to set the Texture or
        /// it will probably crash.
        /// </summary>
        internal DrawCommand()
        {
            clear();
        }

        /// <summary>
        /// Wipes a DrawCommand of all of its values.
        /// </summary>
        internal void clear()
        {
            Texture = null;
            Position = Vector2.Zero;
            Destination = Rectangle.Empty;
            UseDestination = false;
            Rotation = 0.0f;
            CoordinateType = CoordinateTypeEnum.RELATIVE;
            Depth = 0.0f;
            Color = Color.White;
            ImageIndex = 0;
            Effects = SpriteEffects.None;
            Centered = true;
            Scale = 1.0f;
        }

        /// <summary>
        /// Ctor for a DrawCommand with the very basic information to draw.
        /// </summary>
        internal DrawCommand(GameTexture texture, Vector2 position, float depth)
        {
            clear();
            Texture = texture;
            Position = position;
            Depth = depth;
        }

        /// <summary>
        /// Method used to set a DrawCommand with many of the most common
        /// parameters.
        /// </summary>
        internal void set(GameTexture texture,
                            int imageIndex,
                            Vector2 position,
                            CoordinateTypeEnum coordType,
                            float depth,
                            bool centered,
                            Color color,
                            float rotation,
                            float scale)
        {
            Texture = texture;
            Position = position;
            UseDestination = false;
            ImageIndex = imageIndex;
            CoordinateType = coordType;
            Depth = depth;
            Centered = centered;
            Effects = SpriteEffects.None;
            Color = color;
            Rotation = rotation;
            Scale = scale;
        }

        /// <summary>
        /// Converts floating point positions into integers.
        /// </summary>
        /// <param name="v">Vector to convert.</param>
        /// <returns>v with integer X and Y values.</returns>
        internal static Vector2 discretize(Vector2 v)
        {
            return new Vector2((int)v.X, (int)v.Y);
        }

        internal static Rectangle getDestRectangle(Vector2 pos, float scale, Rectangle src)
        {
            return new Rectangle((int)pos.X,
                                 (int)pos.Y,
                                 (int)(scale * src.Width),
                                 (int)(scale * src.Height));
        }

        /// <summary>
        /// Sends the DrawCommand to the SpriteBatch to render with the specified
        /// parameters.  In a multithreaded implementation, should only be called by
        /// the RenderThread.
        /// </summary>
        /// <param name="camPosition">Camera position to draw relative to.</param>
        internal void draw(Vector2 camPosition)
        {
            if (this.CoordinateType == CoordinateTypeEnum.RELATIVE)
            {
                this.Position -= camPosition;
                //this.Destination.Offset(-(int)camPosition.X, -(int)camPosition.Y);
                this.Destination = new Rectangle(this.Destination.X - (int)camPosition.X,
                                                 this.Destination.Y - (int)camPosition.Y,
                                                 this.Destination.Width,
                                                 this.Destination.Height);
            }
            Vector2 origin = Vector2.Zero;
            if (Centered)
            {
                origin.X = (float)Texture.ImageDimensions[ImageIndex].Width / 2.0f;
                origin.Y = (float)Texture.ImageDimensions[ImageIndex].Height / 2.0f;
                origin = discretize(origin);
            }
            if (UseDestination)
            {
                GameTexture.s_spriteBatch.Draw(
                                this.Texture.Texture,
                                this.Destination,
                                this.Texture.ImageDimensions[ImageIndex],
                                this.Color,
                                this.Rotation,
                                origin,
                                this.Effects,
                                1.0f - this.Depth); // we got the convention backwards
            }
            else
            {
                GameTexture.s_spriteBatch.Draw(
                              this.Texture.Texture,
                              //discretize(this.Position),
                              getDestRectangle(this.Position,this.Scale,this.Texture.ImageDimensions[this.ImageIndex]),
                              this.Texture.ImageDimensions[this.ImageIndex],
                              this.Color,
                              this.Rotation,
                              origin,
                              //this.Scale,
                              this.Effects,
                              1.0f - this.Depth); // we got the convention backwards
            }
        }
    }
}
