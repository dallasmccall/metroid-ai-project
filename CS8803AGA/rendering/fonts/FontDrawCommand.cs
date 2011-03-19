using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CS8803AGA
{
    /// <summary>
    /// Much like a texture DrawCommand, an encapsulation of an instruction
    /// to the Engine to render text to the screen.
    /// </summary>
    internal class FontDrawCommand
    {
        public SpriteFont SpriteFont { get; set; }
        public String Text { get; set; }
        public Vector2 Position { get; set; }
        internal CoordinateTypeEnum CoordinateType { get; set; }
        public Color Color { get; set; }
        public float Rotation { get; set; }
        public Vector2 Origin { get; set; }
        public float Scale { get; set; }
        public SpriteEffects Effects { get; set; }
        public float Depth { get; set; }
        protected SpriteBatch m_spriteBatch;

        public FontDrawCommand(SpriteBatch spriteBatch)
        {
            m_spriteBatch = spriteBatch;
            clear();
        }

        internal void clear()
        {
            SpriteFont = null;
            Text = "";
            Position = Vector2.Zero;
            CoordinateType = CoordinateTypeEnum.ABSOLUTE;
            Color = Color.White;
            Rotation = 0.0f;
            Origin = Vector2.Zero;
            Scale = 1.0f;
            Effects = SpriteEffects.None;
            Depth = 0.0f;
        }

        public void set(SpriteFont font,
                        String text,
                        Vector2 position,
                        Color color,
                        float rotation,
                        Vector2 origin,
                        float scale,
                        SpriteEffects effects,
                        float depth)
        {
            SpriteFont = font;
            Text = text;
            Position = position;
            Color = color;
            Rotation = rotation;
            Origin = origin;
            Scale = scale;
            Effects = effects;
            Depth = depth;
        }

        /// <summary>
        /// Should only be called by the Render Thread.
        /// </summary>
        public void draw(Vector2 camPosition)
        {
            if (this.CoordinateType == CoordinateTypeEnum.RELATIVE)
            {
                this.Position -= camPosition;
            }
            m_spriteBatch.DrawString(SpriteFont,
                                    Text,
                                    Position,
                                    Color,
                                    Rotation,
                                    Origin,
                                    Scale,
                                    Effects,
                                    1.0f - Depth); // we got the convention backwards
        }
    }
}
