using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace CS8803AGAGameLibrary.actions
{
    /// <summary>
    /// Information about an area on the sprite being under attack.
    /// </summary>
    public class ActionInfoAttack : AActionInfo
    {
        /// <summary>
        /// Region used by the Action in the sprite, relative to the
        /// draw position (usually the center).
        /// </summary>
        [Description("Area where Attack should be counted, relative to draw position (usually center).")]
        public Rectangle location { get; set; }
    }
}
