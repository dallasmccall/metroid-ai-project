using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace MetroidAIGameLibrary
{
    /// <summary>
    /// Information about a sprite sheet with world decorations, terrain, etc
    /// </summary>
    public class DecorationSetInfo
    {
        /// <summary>
        /// Path to the image file this references
        /// </summary>
        [Description("Path to the image file this references")]
        public string assetPath { get; set; }

        /// <summary>
        /// Information about each decoration on the sheet; key is used to
        /// look up that element in code
        /// </summary>
        [Editor(typeof(DictionaryTypeEditor<string,DecorationInfo>),typeof(UITypeEditor))]
        public Dictionary<string, DecorationInfo> decorations { get; set; }

        public DecorationSetInfo()
        {
            decorations = new Dictionary<string, DecorationInfo>();
        }
    }
}
