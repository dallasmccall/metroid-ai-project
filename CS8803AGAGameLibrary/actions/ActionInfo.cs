using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.ComponentModel;

namespace MetroidAIGameLibrary.actions
{
    /// <summary>
    /// Contains metadata for attaching an in-game Action to an Animation.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class AActionInfo
    {
        public override string ToString()
        {
            return String.Format("{0}", this.GetType().Name);
        }
    }
}
