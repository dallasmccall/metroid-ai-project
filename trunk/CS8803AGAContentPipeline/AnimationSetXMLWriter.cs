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

using TWrite = CS8803AGAGameLibrary.AnimationSetXML;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace CS8803AGAContentPipeline
{
    /// <summary>
    /// See AnimationSetXML in the GameLibrary.
    /// 
    /// This class compiles an AnimationSetXML.xml file (confusing, I know)
    /// into a binary for a true AnimationSet (not an AnimationSetXML).
    /// 
    /// It is probably easier to just create a real AnimationSet, but if you
    /// want to read through this code and try to use it you are more than
    /// welcome to do so.
    /// </summary>
    [ContentTypeWriter]
    public class AnimationSetXMLWriter : ContentTypeWriter<TWrite>
    {
        protected int m_tempCounter = 0;

        protected override void Write(ContentWriter output, TWrite value)
        {
            AnimationSet aset = new AnimationSet();
            aset.anims = new Animation[value.anims.Count];

            // parse global set properties which can be overriden
            int globalFrameHeight = 32;
            int globalFrameWidth = 32;
            int globalDuration = 5;
            int globalOffsetX = 0;
            int globalOffsetY = 0;

            if (value.props != null)
            {
                if (value.props.ContainsKey("spriteHeight"))
                    globalFrameHeight = Int32.Parse(value.props["spriteHeight"]);
                if (value.props.ContainsKey("spriteWidth"))
                    globalFrameWidth = Int32.Parse(value.props["spriteWidth"]);
                if (value.props.ContainsKey("duration"))
                    globalDuration = Int32.Parse(value.props["duration"]);
                if (value.props.ContainsKey("offsetX"))
                    globalOffsetX = Int32.Parse(value.props["offsetX"]);
                if (value.props.ContainsKey("offsetY"))
                    globalOffsetY = Int32.Parse(value.props["offsetY"]);
            }

            int ySoFar = 0;

            // for each property animation, make a real animation
            for(int i = 0; i < value.anims.Count; ++i)
            {
                AnimationXML pa = value.anims[i];

                // parse animation level-only properties
                int priority = 0;
                bool loops = true;
                int idleFrame = 0;
                string whenDone = "idle";
                int yStart = ySoFar;

                if (pa.props != null)
                {
                    if (pa.props.ContainsKey("priority"))
                        priority = Int32.Parse(pa.props["priority"]);
                    if (pa.props.ContainsKey("loops"))
                        loops = Boolean.Parse(pa.props["loops"]);
                    if (pa.props.ContainsKey("idleFrame"))
                        idleFrame = Int32.Parse(pa.props["idleFrame"]);
                    if (pa.props.ContainsKey("whenDone"))
                        whenDone = pa.props["whenDone"];
                    if (pa.props.ContainsKey("yStart"))
                        yStart = Int32.Parse(pa.props["yStart"]);
                }

                // parse animation level properties which can be overriden
                int animFrameWidth = globalFrameWidth;
                int animFrameHeight = globalFrameHeight;
                int animDuration = globalDuration;
                int animOffsetX = globalOffsetX;
                int animOffsetY = globalOffsetY;

                if (pa.props != null)
                {
                    if (pa.props.ContainsKey("spriteWidth"))
                        animFrameWidth = Int32.Parse(pa.props["spriteWidth"]);
                    if (pa.props.ContainsKey("spriteHeight"))
                        animFrameHeight = Int32.Parse(pa.props["spriteHeight"]);
                    if (pa.props.ContainsKey("duration"))
                        animDuration = Int32.Parse(pa.props["duration"]);
                    if (pa.props.ContainsKey("offsetX"))
                        animOffsetX = Int32.Parse(pa.props["offsetX"]);
                    if (pa.props.ContainsKey("offsetY"))
                        animOffsetY = Int32.Parse(pa.props["offsetY"]);
                }

                // make the animation with the properties
                Animation anim = 
                    new Animation(
                        pa.name,
                        pa.numSprites,
                        priority,
                        loops,
                        idleFrame,
                        whenDone);
                aset.anims[i] = anim;

                int xSoFar = 0;
                ySoFar = yStart;
                for (int j = 0; j < pa.numSprites; ++j)
                {
                    anim.sprites[j] = new Sprite();

                    // initialize members from up in hierarchy
                    int spriteHeight = animFrameHeight;
                    int spriteWidth = animFrameWidth;
                    int duration = animDuration;
                    int offsetX = animOffsetX;
                    int offsetY = animOffsetY;

                    string action = "";
                    int x = xSoFar;
                    int y = ySoFar;

                    // if we have a special properties for this sprite, get them
                    if (pa.sprites.ContainsKey(j))
                    {
                        Dictionary<string, string> fa = pa.sprites[j];
                        if (fa.ContainsKey("spriteHeight"))
                            spriteHeight = Int32.Parse(fa["spriteHeight"]);
                        if (fa.ContainsKey("spriteWidth"))
                            spriteWidth = Int32.Parse(fa["spriteWidth"]);
                        if (fa.ContainsKey("duration"))
                            duration = Int32.Parse(fa["duration"]);
                        if (fa.ContainsKey("offsetX"))
                            offsetX = Int32.Parse(fa["offsetX"]);
                        if (fa.ContainsKey("offsetY"))
                            offsetY = Int32.Parse(fa["offsetY"]);
                        if (fa.ContainsKey("action"))
                            action = fa["action"];
                        if (fa.ContainsKey("x"))
                            x = Int32.Parse(fa["x"]);
                        if (fa.ContainsKey("y"))
                            y = Int32.Parse(fa["y"]);
                    }

                    Sprite f = anim.sprites[j];
                    f.box = new Rectangle(x, y, spriteWidth, spriteHeight);
                    f.duration = duration;
                    //f.action = action;
                    f.offset = new Vector2(offsetX, offsetY);

                    xSoFar += spriteWidth;
                }

                ySoFar += animFrameHeight;

            }

            // very hackish - at compile time, output an XML version of AnimationSet
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(String.Format("test{0}.xml", m_tempCounter), settings))
            {
                IntermediateSerializer.Serialize(writer, aset, null);
                m_tempCounter++;
            }

            // now, actually write all that to binary
            output.Write(aset.anims.Length); // # anims
            foreach (Animation a in aset.anims)
            {
                output.Write(a.name);
                output.Write(a.sprites.Length); // # sprites
                output.Write(a.priority);
                output.Write(a.loops);
                output.Write(a.idleFrame);
                output.Write(a.whenDone);
                foreach (Sprite f in a.sprites)
                {
                    output.Write(f.box.X);
                    output.Write(f.box.Y);
                    output.Write(f.box.Width);
                    output.Write(f.box.Height);
                    output.Write(f.duration);
                    //output.Write(f.action);
                    output.Write(f.offset);
                }
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            // TODO: change this to the name of your ContentTypeReader
            // class which will be used to load this data.
            return typeof(CS8803AGAGameLibrary.AnimationSetContentReader).AssemblyQualifiedName;
        }
    }
}
