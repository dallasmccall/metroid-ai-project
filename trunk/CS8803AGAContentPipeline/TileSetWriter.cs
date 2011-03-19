using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using CS8803AGAGameLibrary;

// TODO: replace this with the type you want to write out.
//using TWrite = CS8803AGAGameLibrary.TileSet;

namespace CS8803AGAContentPipeline
{
    /*
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
    [ContentTypeWriter]
    public class TileSetWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            output.Write(value.assetPath);
            output.Write(value.tileWidth);
            output.Write(value.tileHeight);
            output.Write(value.columnsOnSpritesheet);
            
            int numTiles = value.tileInfos.Length;
            output.Write(numTiles);
            for (int i = 0; i < numTiles; ++i)
            {
                writeTileInfo(output, value.tileInfos[i]);
            }
        }

        protected void writeTileInfo(ContentWriter output, TileInfo ti)
        {
            output.Write(ti.passable);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return typeof(CS8803AGAGameLibrary.TileSetContentReader).AssemblyQualifiedName;
        }
    }
     * */
}
