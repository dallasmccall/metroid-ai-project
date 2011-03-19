using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace CS8803AGAGameLibrary
{
    /// <summary>
    /// Metadata about a TileSet
    /// </summary>
    public class TileSet
    {
        /// <summary>
        /// Path to image file with the tiles
        /// </summary>
        public string assetPath { get; set; }

        /// <summary>
        /// Width of each tile
        /// </summary>
        public int tileWidth { get; set; }

        /// <summary>
        /// Height of each tile
        /// </summary>
        public int tileHeight { get; set; }

        /// <summary>
        /// Number of columns of tiles on the sprite sheet
        /// </summary>
        public int columnsOnSpritesheet { get; set; }

        /// <summary>
        /// A TileInfo for each tile, left-to-right then top-to-bottom
        /// </summary>
        public TileInfo[] tileInfos { get; set; }

        public TileSet()
        {
            tileInfos = new TileInfo[0];
        }

        public TileSet(int numTiles)
        {
            tileInfos = new TileInfo[numTiles];
            for (int i = 0; i < numTiles; ++i)
            {
                tileInfos[i] = new TileInfo();
            }
        }

        /// <summary>
        /// Gets bounding areas for all tiles in the spritesheet,
        /// left-to-right then top-to-bottom
        /// </summary>
        /// <returns>Bounding areas for all tiles</returns>
        public Rectangle[] getSpriteSheetSourceRectangles()
        {
            Rectangle[] rects = new Rectangle[tileInfos.Length];
            for (int i = 0; i < tileInfos.Length; ++i)
            {
                rects[i] = new Rectangle(
                    (i % columnsOnSpritesheet) * tileWidth,
                    (i / columnsOnSpritesheet) * tileHeight,
                    tileWidth,
                    tileHeight);
            }
            return rects;
        }
    }

    /*public class TileSetContentReader : ContentTypeReader<TileSet>
    {

        protected TileInfo readTileInfo(ContentReader input)
        {
            TileInfo ti = new TileInfo();
            ti.passable = input.ReadBoolean();

            return ti;
        }

        protected override TileSet Read(ContentReader input, TileSet existingInstance)
        {
            existingInstance = new TileSet();
            existingInstance.assetPath = input.ReadString();
            existingInstance.tileWidth = input.ReadInt32();
            existingInstance.tileHeight = input.ReadInt32();
            existingInstance.columnsOnSpritesheet = input.ReadInt32();

            int numTiles = input.ReadInt32();
            existingInstance.tileInfos = new TileInfo[numTiles];
            for (int i = 0; i < numTiles; ++i)
            {
                existingInstance.tileInfos[i] = readTileInfo(input);
            }

            return existingInstance;
            
        }
    }*/
}
