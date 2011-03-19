using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MetroidAIGameLibrary
{
    /// <summary>
    /// Encapsulation of data needed to load a Decoration from a spritesheet
    /// </summary>
    [TypeConverter(typeof(SmartExpandableConverter<DecorationInfo>))]
    public class DecorationInfo
    {
        /// <summary>
        /// Name of decoration, currently unused
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Area on sprite sheet to render as the decoration
        /// </summary>
        public Rectangle graphic { get; set; }

        /// <summary>
        /// Area on sprite sheet which should have a collision box
        /// </summary>
        public Rectangle collision { get; set; }

        /// <summary>
        /// Currently unused
        /// </summary>
        [ContentSerializer(Optional=true)]
        public bool tileLock { get; set; }
    }
}
