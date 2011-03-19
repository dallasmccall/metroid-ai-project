using System;
using Microsoft.Xna.Framework;

namespace CSharpQuadTree
{
    public interface IQuadObject
    {
        DoubleRect Bounds { get; }
        event EventHandler BoundsChanged;
    }
}