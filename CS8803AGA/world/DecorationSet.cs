using System.Collections.Generic;
using System.Linq;
using CS8803AGAGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CS8803AGA
{
    /// <summary>
    /// A factory for constructing Decorations
    /// </summary>
    public class DecorationSet
    {
        private GameTexture m_texture;
        private Dictionary<string, DecorationInfo> m_infoLookup;
        private Dictionary<string, int> m_indices;

        private DecorationSet()
        {
            // nch
        }

        public static DecorationSet construct(string assetPath)
        {
            return construct(GlobalHelper.loadContent<DecorationSetInfo>(assetPath));
        }

        public static DecorationSet construct(DecorationSetInfo dsi)
        {
            // create source rectangles
            int numDecorations = dsi.decorations.Count;
            Rectangle[] dims = new Rectangle[numDecorations];

            // create index lookup
            Dictionary<string, int> indices = new Dictionary<string,int>();

            int counter = 0;
            foreach (DecorationInfo di in dsi.decorations.Values)
            {
                dims[counter] = new Rectangle(
                    di.graphic.X, di.graphic.Y, di.graphic.Width, di.graphic.Height);
                indices[di.name] = counter;
                counter++;
            }

            DecorationSet ds = new DecorationSet();
            ds.m_texture = new GameTexture(dsi.assetPath, dims);
            ds.m_infoLookup = dsi.decorations;
            ds.m_indices = indices;

            return ds;
        }

        public int getSize()
        {
            return this.m_infoLookup.Count;
        }

        public Decoration makeDecoration(int index, Vector2 collisionCorner)
        {
            // TODO Make this less expensive
            return makeDecoration(this.m_infoLookup.Keys.ToList<string>()[index], collisionCorner);
        }

        public Decoration makeDecoration(int index, Vector2 collisionCorner, Color tint)
        {
            // TODO Make this less expensive
            return makeDecoration(this.m_infoLookup.Keys.ToList<string>()[index], collisionCorner, tint);
        }

        public Decoration makeDecoration(string name, Vector2 collisionCorner)
        {
            DecorationInfo di = m_infoLookup[name];
            Vector2 drawPos =
                new Vector2(collisionCorner.X + di.graphic.X - di.collision.X,
                            collisionCorner.Y + di.graphic.Y - di.collision.Y);
            Decoration d = new Decoration(m_texture, m_indices[name], drawPos, di);
            return d;
        }

        public Decoration makeDecoration(string name, Vector2 collisionCorner, Color tint)
        {
            DecorationInfo di = m_infoLookup[name];
            Vector2 drawPos =
                new Vector2(collisionCorner.X + di.graphic.X - di.collision.X,
                            collisionCorner.Y + di.graphic.Y - di.collision.Y);
            Decoration d = new Decoration(m_texture, m_indices[name], drawPos, di, tint);
            return d;
        }
    }
}
