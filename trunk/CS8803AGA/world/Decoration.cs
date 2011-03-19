using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using CS8803AGAGameLibrary;
using Microsoft.Xna.Framework.Graphics;
using CS8803AGA.collision;
using CS8803AGA.engine;

namespace CS8803AGA
{
    /// <summary>
    /// A piece of scenery in the game world
    /// </summary>
    public class Decoration : ICollidable
    {
        private Vector2 m_drawPosition;
        private int m_drawIndex;          // an index into the graphic for this decoration
        private GameTexture m_texture;    // graphic containing decorations of this set

        private Color m_tint;             // a tint to put on the decoration

        private Collider m_collider;

        /// <summary>
        /// Create a new decoration
        /// </summary>
        /// <param name="decorationSetTexture">Texture containing this decoration's graphic</param>
        /// <param name="indexNumber">An index into the texture, which finds this decoration's location in it</param>
        /// <param name="drawPos">Where in the area the decoration should be drawn</param>
        /// <param name="di">XML information about properties of the Decoration (load from Content)</param>
        /// <param name="tint">Color tint to apply to the graphic</param>
        public Decoration(GameTexture decorationSetTexture, int indexNumber, Vector2 drawPos, DecorationInfo di, Color tint)
        {
            this.m_texture = decorationSetTexture;
            this.m_drawIndex = indexNumber;
            this.m_drawPosition = drawPos;

            Rectangle bounds = new Rectangle(
                di.collision.X - di.graphic.X + (int)drawPos.X,
                di.collision.Y - di.graphic.Y + (int)drawPos.Y,
                di.collision.Width,
                di.collision.Height);
            this.m_collider = new Collider(this, bounds, ColliderType.Scenery);

            this.m_tint = tint;
        }

        public Decoration(GameTexture decorationSetTexture, int indexNumber, Vector2 drawPos, DecorationInfo di)
            : this(decorationSetTexture, indexNumber, drawPos, di, new Color((float)RandomManager.get().NextDouble()/5f + 0.80f,
                                                                                (float)RandomManager.get().NextDouble() / 5f + 0.80f,
                                                                                (float)RandomManager.get().NextDouble() / 5f + 0.80f))
        {
            // nch
        }

        #region Drawing Methods

        /// <summary>
        /// Normal draw method to be called during gameplay
        /// </summary>
        public void draw()
        {
            //float depth = GameplayManager.ActiveArea.getDrawDepth(this.getCollider().Bounds);
            float depth = Constants.DepthGameplayDecorationsBehind;

            DrawCommand td = DrawBuffer.getInstance().DrawCommands.pushGet();
            td.set(m_texture, m_drawIndex, m_drawPosition, CoordinateTypeEnum.RELATIVE, depth, false, m_tint, 0, 1.0f);
        }

        /// <summary>
        /// Scaled draw method for use in maps, minimaps, etc.
        /// </summary>
        /// <param name="offset">Pixel-position where the top left corner of the decoration should be drawn</param>
        /// <param name="scale">Amount the size of the graphic will be multiplied by when drawing</param>
        /// <param name="depth">z-Depth of the image</param>
        public void drawMap(Vector2 offset, float scale, float depth)
        {
            DrawCommand td = DrawBuffer.getInstance().DrawCommands.pushGet();
            td.set(m_texture, m_drawIndex, m_drawPosition * scale + offset, CoordinateTypeEnum.ABSOLUTE, depth + 0.0001f, false, m_tint, 0, scale);
        }

        #endregion

        #region Collidable Members

        public Collider getCollider()
        {
            return m_collider;
        }

        public Vector2 DrawPosition
        {
            get
            {
                return m_drawPosition;
            }
            set
            {
                m_drawPosition = value;
            }
        }

        #endregion

        #region IGameObject Members

        public bool isAlive()
        {
            return true;
        }

        public void update()
        {
            // nch
        }

        #endregion
    }
}
