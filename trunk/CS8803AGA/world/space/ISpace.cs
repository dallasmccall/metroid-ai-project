using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CS8803AGA.world.space
{
    /// <summary>
    /// Interface for the structure representing rooms on a grid (Space).
    /// Contains information about which mission terminals were mapped to
    /// those rooms and connection types between rooms.
    /// </summary>
    interface ISpace
    {
        bool IsAreaFree(Point coord);
        ScreenConstructionParameters GetParameters(Point coord);
        IEnumerable<String> GetQualifiers(Point coord);
        Connection? GetConnection(Point sourceCoord, Point destCoord);

        Dictionary<CoordPair, Connection> OpenConnections { get; }

        void AddScreen(ScreenConstructionParameters scp, String missionNodeID);
        void AddQualifier(Point coord, String qualifier);
        void ConnectOneWay(Point sourceCoord, Point destCoord, Connection connection);
        void ConnectTwoWay(Point sourceCoord, Point destCoord, Connection connection);

        ISpace DeepCopy();
    }

    struct CoordPair
    {
        public Point SourceCoord { get; set; }
        public Point DestCoord { get; set; }

        public CoordPair(Point sourceCoord, Point destCoord) : this()
        {
            SourceCoord = sourceCoord;
            DestCoord = destCoord;
        }

        public CoordPair Reverse
        {
            get {
                CoordPair cp = new CoordPair();
                cp.SourceCoord = DestCoord;
                cp.DestCoord = SourceCoord;
                return cp;
            }
        }

        public Direction Facing
        {
            get { return DirectionUtils.GetDirection(SourceCoord, DestCoord); }
        }

        public override int GetHashCode()
        {
            return
                SourceCoord.X.GetHashCode() ^ SourceCoord.Y.GetHashCode() ^
                DestCoord.X.GetHashCode() ^ DestCoord.Y.GetHashCode();
        }
    }
}