using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CS8803AGAGameLibrary
{
    /// <summary>
    /// This class is deprecated because it doesn't play well with the editor.
    /// 
    /// Its purpose was to generate AnimationSet files more easily because it
    /// used hierarchial assumptions so that information about each sprite, row,
    /// etc didn't have to be filled in by hand; if information about sprite width
    /// was filled in for the row, etc, it assumed that width for each sprite in the
    /// row unless that value was overridden for an individual sprite.
    /// 
    /// If you want to use this, you have to make the XML by hand.  I recommend
    /// looking at some of the Content/Animations/Data files marked with "OLD" as
    /// examples.  Then see AnimationSetXMLWriter.cs in the ContentPipeline project
    /// as that file converted these into AnimationSets for game.  Currently it also
    /// has a little hack which will dump them into XMLs of true AnimationSets at
    /// build time so you can tweak them in that form if you'd like.
    /// 
    /// Also, it doesn't support actions and some other things that have changed, so
    /// you may have to make them Optional and do some tweaking to get these to work.
    /// </summary>
    public class AnimationXML
    {
        public string name { get; set; }
        public int numSprites { get; set; }
        public Dictionary<int, Dictionary<string, string>> sprites { get; set; }
        public Dictionary<String, String> props { get; set; }

        public AnimationXML()
        {
            sprites = new Dictionary<int, Dictionary<string, string>>();
            props = new Dictionary<string, string>();
        }

        public AnimationXML(string name, int numSprites)
        {
            this.name = name;
            this.numSprites = numSprites;
            this.sprites = new Dictionary<int, Dictionary<string, string>>();
            this.props = new Dictionary<string, string>();
        }
    }

    public class AnimationSetXML
    {
        public Dictionary<string, string> props { get; set; }
        public List<AnimationXML> anims { get; set; }

        public AnimationSetXML()
        {
            anims = new List<AnimationXML>();
            props = new Dictionary<string,string>();
        }
    }
}
